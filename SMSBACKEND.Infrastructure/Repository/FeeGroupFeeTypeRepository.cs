
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using static Common.Utilities.Enums;

namespace Infrastructure.Repository
{
    public class FeeGroupFeeTypeRepository : BaseRepository<FeeGroupFeeType, int>, IFeeGroupFeeTypeRepository
    {
        public FeeGroupFeeTypeRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }
        public async Task<(List<FeeGroupWithTypesModel> feeGroupFeeTypeModels, int lastId)> GetFeeGroupFeeType(FilterParams filters)
        {
            var feeGroups = await DbContext.FeeGroup
            .Include(fg => fg.FeeGroupFeeTypes)
                .ThenInclude(fgt => fgt.FeeType)
            .Where(fg => !fg.IsDeleted && fg.FeeGroupFeeTypes.Count > 0)
            .Select(fg => new FeeGroupWithTypesModel
            {
                FeeGroupIds = fg.FeeGroupFeeTypes.Select(x=> x.FeeGroupId).ToList(),
                FeeGroupName = fg.Name,
                Description = fg.Description,
                IsSystem = fg.IsSystem,
                FeeTypeIds = fg.FeeGroupFeeTypes.Where(fgt => !fgt.FeeType.IsDeleted).Select(fgt => fgt.FeeTypeId).ToList(),
                FeeGroupFeeTypeIds = fg.FeeGroupFeeTypes.Where(fgt => !fgt.FeeType.IsDeleted).Select(fgt => fgt.Id).ToList(),
                FeeTypes = fg.FeeGroupFeeTypes
                    .Where(fgt => !fgt.FeeType.IsDeleted)
                    .Select(fgt => new FeeTypeDetailModel
                    {
                        FeeTypeId = fgt.FeeType.Id,
                        Name = fgt.FeeType.Name,
                        Code = fgt.FeeType.Code,
                        FeeFrequency = fgt.FeeType.FeeFrequency,
                        IsSystem = fgt.FeeType.IsSystem,
                        Amount = fgt.Amount,
                        DueDate = fgt.DueDate,
                        FineType = fgt.FineType.ToString(),
                        FineAmount = fgt.FineAmount,
                        FinePercentage = fgt.FinePercentage,
                        FeeDiscountId = fgt.FeeDiscountId
                    }).ToList()
            })
            .Skip(filters.PaginationParam.PageSize * (filters.PaginationParam.PageIndex - 1))
            .Take(filters.PaginationParam.PageSize)
            .AsNoTracking()
            .ToListAsync();

            var lastId = await DbContext.FeeGroup
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            return (feeGroups, lastId);
        }
        public async Task<int> SaveFeeGroupFeeType(FeeGroupFeeTypeRequest model)
        {
            if (model.FeeTypes == null || !model.FeeTypes.Any())
                return 0;

            var inputFeeTypeIds = model.FeeTypes.Select(ft => ft.FeeTypeId).Distinct().ToList();

            // Fetch existing mappings for this FeeGroup
            var existingMappings = await DbContext.FeeGroupFeeType.IgnoreQueryFilters().Where(fgt => fgt.FeeGroupId == model.FeeGroupIds)
                .ToListAsync();

            var activeFeeTypeIds = existingMappings.Where(x => !x.IsDeleted).Select(x => x.FeeTypeId).ToList();

            // Identify new entries
            var newFeeTypes = model.FeeTypes.Where(ft => !activeFeeTypeIds.Contains(ft.FeeTypeId)).ToList();

            foreach (var ft in newFeeTypes)
            {
                var revived = existingMappings.FirstOrDefault(x => x.FeeTypeId == ft.FeeTypeId &&
                x.FeeGroupId == model.FeeGroupIds && x.IsDeleted);
                if (revived != null)
                {
                    revived.IsDeleted = false;
                    revived.Amount = ft.Amount;
                    revived.DueDate = ft.DueDate;
                    revived.FineType = Enum.TryParse<FinePolicyType>(ft.FineType, out var fineEnum) ? fineEnum : FinePolicyType.None;
                    revived.FineAmount = ft.FineAmount;
                    revived.FeeDiscountId = ft.FeeDiscountId;
                    revived.FinePercentage = ft.FinePercentage;
                    revived.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    DbContext.FeeGroupFeeType.Add(new FeeGroupFeeType
                    {
                        FeeGroupId = model.FeeGroupIds,
                        FeeTypeId = ft.FeeTypeId,
                        FeeDiscountId = ft.FeeDiscountId,
                        Amount = ft.Amount,
                        DueDate = ft.DueDate,
                        FineType = Enum.TryParse<FinePolicyType>(ft.FineType, out var fineTypeEnum) ? fineTypeEnum : FinePolicyType.None,
                        FineAmount = ft.FineAmount,
                        FinePercentage = ft.FinePercentage,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            // Identify and update existing entries
            foreach (var existing in existingMappings)
            {
                var updated = model.FeeTypes.FirstOrDefault(ft => ft.FeeTypeId == existing.FeeTypeId);
                if (updated != null)
                {
                    bool isChanged =
                        existing.Amount != updated.Amount ||
                        existing.DueDate != updated.DueDate ||
                        existing.FineType.ToString() != updated.FineType ||
                        existing.FineAmount != updated.FineAmount ||
                        existing.FeeDiscountId != updated.FeeDiscountId ||
                        existing.FinePercentage != updated.FinePercentage;

                    if (isChanged)
                    {
                        existing.Amount = updated.Amount;
                        existing.DueDate = updated.DueDate;
                        existing.FineType = Enum.TryParse<FinePolicyType>(updated.FineType, out var fineEnum) ? fineEnum : FinePolicyType.None;
                        existing.FineAmount = updated.FineAmount;
                        existing.FeeDiscountId = updated.FeeDiscountId;
                        existing.FinePercentage = updated.FinePercentage;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            // Identify and remove deleted entries
            var idsToSoftDelete = existingMappings
                .Where(existing => !inputFeeTypeIds.Contains(existing.FeeTypeId) && !existing.IsDeleted).ToList();

            foreach (var item in idsToSoftDelete)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }

            return await DbContext.SaveChangesAsync();
        }


    }
}
