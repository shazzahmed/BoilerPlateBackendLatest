using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DTO.Request;
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class FineWaiverRepository : BaseRepository<FineWaiver, int>, IFineWaiverRepository
    {
        private readonly ISqlServerDbContext _context;
        private readonly ILogger<FineWaiverRepository> _logger;

        public FineWaiverRepository(ISqlServerDbContext context, ILogger<FineWaiverRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FineWaiver> RequestWaiverAsync(FineWaiverRequest request)
        {
            var waiver = new FineWaiver
            {
                FeeAssignmentId = request.FeeAssignmentId,
                StudentId = request.StudentId,
                OriginalFineAmount = request.FineAmount,
                WaiverAmount = request.WaiverAmount,
                Reason = request.Reason,
                Status = "Pending",
                RequestedBy = "CurrentUser", // TODO: Get from HttpContext
                RequestedDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = "CurrentUser"
            };

            await _context.Set<FineWaiver>().AddAsync(waiver);
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Fine waiver requested - ID: {Id}, Student: {StudentId}, Amount: {Amount}", 
                waiver.Id, waiver.StudentId, waiver.WaiverAmount);

            // Reload with navigation properties
            var savedWaiver = await _context.Set<FineWaiver>()
                .Include(w => w.Student).ThenInclude(s => s.StudentInfo)
                .Include(w => w.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                .FirstOrDefaultAsync(w => w.Id == waiver.Id);

            return savedWaiver ?? waiver;
        }

        public async Task<FineWaiver> ApproveWaiverAsync(FineWaiverApprovalRequest request)
        {
            var waiver = await _context.Set<FineWaiver>()
                .Include(w => w.FeeAssignment)
                .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted);

            if (waiver == null)
                throw new Exception("Fine waiver not found");

            if (waiver.Status != "Pending")
                throw new Exception("Only pending waivers can be approved or rejected");

            waiver.Status = request.IsApproved ? "Approved" : "Rejected";
            waiver.ApprovedBy = "CurrentUser"; // TODO: Get from HttpContext
            waiver.ApprovalDate = DateTime.Now;
            waiver.ApprovalNote = request.ApprovalNote;
            waiver.UpdatedAt = DateTime.Now;
            waiver.UpdatedBy = "CurrentUser";

            // If approved, reduce the fine amount in fee assignment
            if (request.IsApproved && waiver.FeeAssignment != null)
            {
                waiver.FeeAssignment.AmountFine -= waiver.WaiverAmount;
                if (waiver.FeeAssignment.AmountFine < 0)
                {
                    waiver.FeeAssignment.AmountFine = 0;
                }
                waiver.FeeAssignment.UpdatedAt = DateTime.Now;
                waiver.FeeAssignment.UpdatedBy = "CurrentUser";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Fine waiver {Status} - ID: {Id}, Waiver Amount: {Amount}", 
                waiver.Status, waiver.Id, waiver.WaiverAmount);

            // Reload with all navigation properties
            var updatedWaiver = await _context.Set<FineWaiver>()
                .Include(w => w.Student).ThenInclude(s => s.StudentInfo)
                .Include(w => w.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                .FirstOrDefaultAsync(w => w.Id == waiver.Id);

            return updatedWaiver ?? waiver;
        }

        public async Task<List<FineWaiver>> GetPendingWaiversAsync()
        {
            return await _context.Set<FineWaiver>()
                .Include(w => w.Student).ThenInclude(s => s.StudentInfo)
                .Include(w => w.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                .Where(w => w.Status == "Pending" && !w.IsDeleted)
                .OrderByDescending(w => w.RequestedDate)
                .ToListAsync();
        }

        public async Task<List<FineWaiver>> GetWaiversByStudentAsync(int studentId)
        {
            return await _context.Set<FineWaiver>()
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                .Where(w => w.StudentId == studentId && !w.IsDeleted)
                .OrderByDescending(w => w.RequestedDate)
                .ToListAsync();
        }

        public async Task<List<FineWaiver>> GetWaiversByStatusAsync(string status)
        {
            var query = _context.Set<FineWaiver>()
                .Include(w => w.Student).ThenInclude(s => s.StudentInfo)
                .Include(w => w.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                .Include(w => w.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                .Where(w => !w.IsDeleted);

            // If status is "All", don't filter by status
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                query = query.Where(w => w.Status == status);
            }

            return await query
                .OrderByDescending(w => w.RequestedDate)
                .ToListAsync();
        }
    }
}
