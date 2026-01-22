using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    /// <summary>
    /// Repository contract for Section entity
    /// Currently uses only base repository methods
    /// This interface exists for:
    /// - Dependency Injection specificity
    /// - Future extensibility for custom Section queries
    /// - Maintaining consistent repository pattern across the domain
    /// </summary>
    public interface ISectionRepository : IBaseRepository<Section, int>
    {
        // No custom methods currently needed
        // Section operations use standard CRUD from IBaseRepository
    }
}
