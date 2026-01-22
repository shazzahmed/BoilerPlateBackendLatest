
namespace Domain.RepositoriesContracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
