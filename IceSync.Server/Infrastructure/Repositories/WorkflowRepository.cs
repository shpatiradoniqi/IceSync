using IceSync.Server.Domain.Entities;
using IceSync.Server.Domain.Interfaces;
using IceSync.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Server.Infrastructure.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly IceSyncDbContext _context;

        public WorkflowRepository(IceSyncDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Workflow>> GetAllAsync()
        {
            return await _context.Workflows.ToListAsync();
        }

        public async Task<Workflow> GetByIdAsync(int workflowId)
        {
            return await _context.Workflows.FirstOrDefaultAsync(w => w.WorkflowId == workflowId);
        }

        public async Task AddAsync(Workflow workflow)
        {
            await _context.Workflows.AddAsync(workflow);
        }

        public async Task UpdateAsync(Workflow workflow)
        {
            _context.Workflows.Update(workflow);
        }

        public async Task DeleteAsync(int workflowId)
        {
            var workflow = await GetByIdAsync(workflowId);
            if (workflow != null)
            {
                _context.Workflows.Remove(workflow);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}

