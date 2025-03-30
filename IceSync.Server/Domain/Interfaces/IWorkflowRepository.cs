using IceSync.Server.Domain.Entities;

namespace IceSync.Server.Domain.Interfaces
{
    public interface IWorkflowRepository
    {
        Task<IEnumerable<Workflow>> GetAllAsync();
        Task<Workflow> GetByIdAsync(int workflowId);
        Task AddAsync(Workflow workflow);
        Task UpdateAsync(Workflow workflow);
        Task DeleteAsync(int workflowId);
        Task SaveChangesAsync();

    }
}
