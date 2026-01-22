
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface ISubjectService : IBaseService<SubjectModel, Subject, int>
    {
        Task CreateSubject(SubjectModel model);
        Task UpdateSubject(SubjectModel model);
    }
}
