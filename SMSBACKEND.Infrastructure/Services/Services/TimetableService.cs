
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using Infrastructure.Database;

namespace Infrastructure.Services.Services
{
    public class TimetableService : BaseService<TimetableModel, Timetable, int>, ITimetableService
    {
        private readonly ITimetableRepository _timetableRepository;
        private readonly IClassService _classService;
        private readonly ISessionService _sessionService;
        private readonly ISqlServerDbContext _context;

        public TimetableService(
            IMapper mapper,
            ITimetableRepository timetableRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            IClassService classService,
            ISqlServerDbContext context,
            ISessionService sessionService) : base(mapper, timetableRepository, unitOfWork, sseService)
        {
            _timetableRepository = timetableRepository;
            _classService = classService;
            _context = context;
            _sessionService = sessionService;
        }

        public async Task<List<TimetableModel>> GetAllTimetables()
        {
            try
            {
                // Get all timetable entries from database
                var timetableEntries = await _timetableRepository.GetAsync();
                
                if (!timetableEntries.Any())
                {
                    return new List<TimetableModel>();
                }

                // Group entries by ClassSectionId to create timetables
                var groupedEntries = timetableEntries
                    .Where(te => !te.IsDeleted)
                    .GroupBy(te => te.ClassSectionId)
                    .ToList();

                var timetables = new List<TimetableModel>();

                foreach (var group in groupedEntries)
                {
                    var classSectionId = group.Key;
                    var entries = group.OrderBy(e => e.DayOfWeek).ThenBy(e => e.OrderIndex).ToList();

                    // Get class section details
                    var classSectionResult = await _classService.GetClassSectionById(classSectionId);
                    if (!classSectionResult.Success || classSectionResult.Data == null)
                    {
                        continue; // Skip if class section not found
                    }

                    var classSection = mapper.Map<ClassSectionModel>(classSectionResult.Data);

                    // Create timetable model
                    var timetable = new TimetableModel
                    {
                        Id = entries.First().Id, // Use first entry's ID as timetable ID
                        ClassSectionId = classSectionId,
                        ClassName = classSection.ClassName,
                        SectionName = classSection.SectionName,
                        TimetableEntries = entries.Select(entry => new TimetableEntryModel
                        {
                            Id = entry.Id,
                            TimetableId = entry.Id, // Using entry ID as timetable ID
                            DayOfWeek = entry.DayOfWeek,
                            SubjectId = entry.SubjectId,
                            SubjectName = "", // Will be populated from navigation properties if needed
                            TeacherId = entry.TeacherId,
                            TeacherName = "", // Will be populated from navigation properties if needed
                            StartTime = entry.StartTime,
                            EndTime = entry.EndTime,
                            Duration = entry.Duration,
                            RoomNumber = entry.RoomNumber,
                            IsBreak = entry.IsBreak,
                            BreakName = entry.BreakName,
                            OrderIndex = entry.OrderIndex,
                            IsActive = entry.IsActive,
                            IsDeleted = entry.IsDeleted
                        }).ToList(),
                        IsDeleted = false,
                        CreatedAt = entries.First().CreatedAt,
                        UpdatedAt = entries.First().UpdatedAt,
                        SyncStatus = "synced"
                    };

                    timetables.Add(timetable);
                }

                return timetables;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all timetables: {ex.Message}");
            }
        }



        public async Task<TimetableModel> SaveTimetable(TimetableDifferentialUpdateRequest request)
        {
            try
            {
                // Get class section details
                var classSectionResult = await _classService.GetClassSectionById(request.ClassSectionId);
                if (!classSectionResult.Success || classSectionResult.Data == null)
                {
                    throw new Exception("Class section not found");
                }

                var classSection = mapper.Map<ClassSectionModel>(classSectionResult.Data);

                // 1. DELETE entries that are no longer present
                if (request.EntriesToDelete.Any())
                {
                    var entriesToDelete = await _timetableRepository.GetAsync(
                        where: t => request.EntriesToDelete.Contains(t.Id) && !t.IsDeleted
                    );

                    foreach (var entry in entriesToDelete)
                    {
                        entry.IsDeleted = true;
                        entry.UpdatedAt = DateTime.Now;
                        await _timetableRepository.UpdateAsync(entry);
                    }
                }

                // 2. UPDATE existing entries that have changed
                foreach (var updateEntry in request.EntriesToUpdate)
                {
                    if (updateEntry.Id.HasValue)
                    {
                        var existingEntry = await _timetableRepository.GetAsync(updateEntry.Id.Value);
                        if (existingEntry != null && !existingEntry.IsDeleted)
                        {
                            // Update the existing entry
                            existingEntry.DayOfWeek = updateEntry.DayOfWeek;
                            existingEntry.SubjectId = updateEntry.SubjectId;
                            existingEntry.TeacherId = updateEntry.TeacherId;
                            existingEntry.StartTime = updateEntry.StartTime;
                            existingEntry.EndTime = updateEntry.EndTime;
                            existingEntry.Duration = updateEntry.Duration;
                            existingEntry.RoomNumber = updateEntry.RoomNumber;
                            existingEntry.IsBreak = updateEntry.IsBreak;
                            existingEntry.BreakName = updateEntry.BreakName;
                            existingEntry.OrderIndex = updateEntry.OrderIndex;
                            existingEntry.UpdatedAt = DateTime.Now;

                            await _timetableRepository.UpdateAsync(existingEntry);
                        }
                    }
                }

                // Get active session ID
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }

                // 3. ADD new entries
                foreach (var newEntry in request.EntriesToAdd)
                {
                    var timetableEntry = new Timetable
                    {
                        ClassSectionId = request.ClassSectionId,
                        SessionId = activeSession, // Set active session
                        DayOfWeek = newEntry.DayOfWeek,
                        SubjectId = newEntry.SubjectId,
                        TeacherId = newEntry.TeacherId,
                        StartTime = newEntry.StartTime,
                        EndTime = newEntry.EndTime,
                        Duration = newEntry.Duration,
                        RoomNumber = newEntry.RoomNumber,
                        IsBreak = newEntry.IsBreak,
                        BreakName = newEntry.BreakName,
                        OrderIndex = newEntry.OrderIndex,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _timetableRepository.AddAsync(timetableEntry);
                }

                await SaveChanges();

                // Return the updated timetable
                var allEntries = await _timetableRepository.GetAsync(
                    where: t => t.ClassSectionId == request.ClassSectionId && !t.IsDeleted
                );

                var timetableModel = new TimetableModel
                {
                    ClassSectionId = request.ClassSectionId,
                    ClassName = classSection.ClassName,
                    SectionName = classSection.SectionName,
                    TimetableEntries = mapper.Map<List<TimetableEntryModel>>(allEntries)
                };

                return timetableModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to differential update timetable: {ex.Message}");
            }
        }

    }
}

