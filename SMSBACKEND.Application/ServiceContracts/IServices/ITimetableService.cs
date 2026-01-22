
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface ITimetableService : IBaseService<TimetableModel, Timetable, int>
    {
        Task<List<TimetableModel>> GetAllTimetables();
        Task<TimetableModel> SaveTimetable(TimetableDifferentialUpdateRequest request);
    }
}
