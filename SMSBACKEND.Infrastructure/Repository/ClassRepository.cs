
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using static System.Collections.Specialized.BitVector32;
using Microsoft.EntityFrameworkCore;
using static Common.Utilities.Enums;
using Common.DTO.Request;
using AutoMapper;
using System.Collections.Generic;
using Application.ServiceContracts;
using IdentityModel;
using Common.Utilities.StaticClasses;
using Infrastructure.Services.Communication;

namespace Infrastructure.Repository
{
    public class ClassRepository : BaseRepository<Class, int>, IClassRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;
        public ClassRepository(IMapper mapper, ISqlServerDbContext context, ICacheProvider cacheProvider, SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }
        public async Task CreateClassSection(ClassModel model, List<int> SelectedSectionIds)
        {
            try
            {
                var classSections = SelectedSectionIds.Select(sectionId => new ClassSection
                {
                    ClassId = model.Id,
                    SectionId = sectionId,
                    Class = null
                }).OrderBy(x=> x.SectionId).ToList();
                DbContext.ClassSections.AddRange(classSections);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        public async Task UpdateClassSection(ClassModel model, List<int> SelectedSectionIds)
        {
            var classSections = await DbContext.ClassSections.Where(x=> x.ClassId == model.Id && !x.Class.IsDeleted && !x.Section.IsDeleted).ToListAsync();
            var existingIds = classSections.Select(x => x.SectionId).ToList();
            var idsToAdd = SelectedSectionIds.Where(id => !existingIds.Contains(id)).OrderBy(x=> x).ToList();
            var idsToRemove = existingIds.Where(id => !SelectedSectionIds.Contains(id)).ToList();
            foreach (var id in idsToAdd)
            {
                DbContext.ClassSections.Add(new ClassSection
                {
                    ClassId = model.Id,
                    SectionId = id
                });
            }

            // Remove old entries
            foreach (var id in idsToRemove)
            {
                var sectionToRemove = classSections.FirstOrDefault(x => x.SectionId == id);
                if (sectionToRemove != null)
                {
                    DbContext.ClassSections.Remove(sectionToRemove);
                }
            }
        }
        public async Task CreateSubjectClass(ClassModel model, List<int> SelectedSubjectIds)
        {
            try
            {
                var subjectClasses = SelectedSubjectIds.Select(subjectId => new SubjectClass
                {
                    SubjectId = subjectId,
                    ClassId = model.Id,
                    Class = null,
                    Subject = null
                }).OrderBy(x => x.ClassId).ToList();
                DbContext.SubjectClass.AddRange(subjectClasses);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task UpdateSubjectClass(ClassModel model, List<int> SelectedSubjectIds)
        {
            var subjectClasses = await DbContext.SubjectClass.Where(x => x.ClassId == model.Id && !x.Class.IsDeleted).ToListAsync();
            var existingIds = subjectClasses.Select(x => x.SubjectId).ToList();
            var idsToAdd = SelectedSubjectIds.Where(id => !existingIds.Contains(id)).OrderBy(x => x).ToList();
            var idsToRemove = existingIds.Where(id => !SelectedSubjectIds.Contains(id)).ToList();
            foreach (var id in idsToAdd)
            {
                DbContext.SubjectClass.Add(new SubjectClass
                {
                    ClassId = model.Id,
                    SubjectId = id,
                });
            }

            // Remove old entries
            foreach (var id in idsToRemove)
            {
                var subjectToRemove = subjectClasses.FirstOrDefault(x => x.SubjectId == id);
                if (subjectToRemove != null)
                {
                    DbContext.SubjectClass.Remove(subjectToRemove);
                }
            }
        }
        public async Task<(List<ClassTeacherModel> classTeachers, int lastId)> GetClassTeacher()
        {
            var classTeachers = await DbContext.ClassTeacher
                .Include(s => s.Staff)
                .Include(s => s.ClassSection).ThenInclude(s => s.Section)
                .Include(s => s.ClassSection).ThenInclude(s => s.Class)
                .Where(x=> !x.ClassSection.Class.IsDeleted && !x.Staff.IsDeleted && !x.ClassSection.Section.IsDeleted)
                .GroupBy(ct => new { 
                    ct.ClassSectionId,
                    ClassName = ct.ClassSection.Class.Name,
                    SectionName = ct.ClassSection.Section.Name,
                    ClassId = ct.ClassSection.Class.Id,
                    SectionId = ct.ClassSection.Section.Id,
                })    
                .Select(group => new ClassTeacherModel
                {
                    ClassSectionId = group.Key.ClassSectionId,
                    ClassName = group.Key.ClassName,
                    SectionName = group.Key.SectionName,
                    StaffNameList = group.Select(ct => ct.Staff.FirstName + " " + ct.Staff.LastName).ToList(),
                    StaffIds = group.Select(ct => ct.Staff.Id).ToList(),

                })
                .OrderByDescending(l => l.ClassSectionId)
                .AsNoTracking()
                .ToListAsync();
            var lastId = await DbContext.ClassTeacher
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            return (classTeachers, lastId);
        }
        public async Task<int> CreateClassTeacher(ClassTeacherRequest model)
        {
            var existingStaffIds = await DbContext.ClassTeacher
                .Where(ct => ct.ClassSectionId == model.ClassSectionId)
                .Select(ct => ct.StaffId)
                .ToListAsync();

            var inputStaffIds = model.StaffIds.Distinct().ToList();

            var newStaffIds = inputStaffIds
                .Except(existingStaffIds)
                .ToList();

            if (!newStaffIds.Any()) return 0;

            var newRecords = newStaffIds.Select(staffId => new ClassTeacher
            {
                ClassSectionId = model.ClassSectionId,
                StaffId = staffId
            }).ToList();

            await DbContext.ClassTeacher.AddRangeAsync(newRecords);
            return await DbContext.SaveChangesAsync();
        }
        public async Task<int> UpdateClassTeacher(ClassTeacherRequest model)
        {
            var existingRecords = await DbContext.ClassTeacher
                .Where(ct => ct.ClassSectionId == model.ClassSectionId &&
                             !ct.ClassSection.Class.IsDeleted &&
                             !ct.ClassSection.Section.IsDeleted)
                .ToListAsync();

            var existingStaffIds = existingRecords.Select(r => r.StaffId).ToHashSet();
            var inputStaffIds = model.StaffIds.Distinct().ToHashSet();

            var staffToAdd = inputStaffIds.Except(existingStaffIds).ToList();
            var staffToRemove = existingRecords.Where(r => !inputStaffIds.Contains(r.StaffId)).ToList();

            if (staffToAdd.Any())
            {
                var newRecords = staffToAdd.Select(staffId => new ClassTeacher
                {
                    ClassSectionId = model.ClassSectionId,
                    StaffId = staffId
                }).ToList();

                await DbContext.ClassTeacher.AddRangeAsync(newRecords);
            }

            if (staffToRemove.Any())
            {
                DbContext.ClassTeacher.RemoveRange(staffToRemove);
            }

            return await DbContext.SaveChangesAsync();
        }
        public async Task<BaseModel> DeleteClassTeacher(int ClassSectionId)
        {
            var existingRecords = DbContext.ClassTeacher
                .Where(ct => ct.ClassSectionId == ClassSectionId && !ct.ClassSection.Class.IsDeleted && !ct.ClassSection.Section.IsDeleted).ToList();
            if (existingRecords.Any())
            {
                DbContext.ClassTeacher.RemoveRange(existingRecords);
                await DbContext.SaveChangesAsync();
                return new BaseModel { Success = true, Message = "Class teacher deleted successfully." };
            }
            return new BaseModel { Success = true, Message = "Class teacher not exist." };
        }
        public async Task<BaseModel> GetClassSectionById(int ClassSectionId)
        {
            var result = await DbContext.ClassSections
                .Include(s => s.Section)
                .Include(s => s.Class)
                .Where(ct => ct.Id == ClassSectionId && !ct.Class.IsDeleted && !ct.Section.IsDeleted).FirstOrDefaultAsync();
            return new BaseModel { Success = true, Data = result };
        }

    }
}
