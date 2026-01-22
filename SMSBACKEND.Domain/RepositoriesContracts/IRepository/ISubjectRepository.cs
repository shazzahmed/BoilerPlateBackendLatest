using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    /// <summary>
    /// Repository contract for Subject entity
    /// Currently uses only base repository methods
    /// This interface exists for:
    /// - Dependency Injection specificity
    /// - Future extensibility for custom Subject queries
    /// - Maintaining consistent repository pattern across the domain
    /// </summary>
    public interface ISubjectRepository : IBaseRepository<Subject, int>
    {
        // No custom methods currently needed
        // Subject operations use standard CRUD from IBaseRepository
    }
}
