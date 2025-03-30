using IceSync.Server.Domain.Entities;
using IceSync.Server.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IceSync.Server.Infrastructure.Repositories;
using IceSync.Server.Application.Services;
namespace IceSync.Server.Application.Services
{
   
    public class WorkflowService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkflowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var loaderService = scope.ServiceProvider.GetRequiredService<UniversalLoaderService>();
                var workflowRepository = scope.ServiceProvider.GetRequiredService<IWorkflowRepository>();

                var workflowDtos = (await loaderService.GetWorkflowsAsync()).ToList();
                var workflowsFromDb = (await workflowRepository.GetAllAsync()).ToList();

                // Convert DTO to Entity before processing
                var workflowsFromApi = workflowDtos.Select(dto => new Workflow
                {
                    WorkflowId = dto.Id,  
                    WorkflowName = !string.IsNullOrEmpty(dto.Name) ? dto.Name : "Unknown",  
                    IsActive = dto.IsActive,
                    MultiExecBehavior = dto.MultiExecBehavior
                }).ToList();

                // Insert or update workflows
                foreach (var apiWorkflow in workflowsFromApi)
                {
                    var dbWorkflow = workflowsFromDb.FirstOrDefault(w => w.WorkflowId == apiWorkflow.WorkflowId);

                    if (dbWorkflow == null)
                    {
                        await workflowRepository.AddAsync(apiWorkflow);
                    }
                    else
                    {
                        dbWorkflow.WorkflowName = apiWorkflow.WorkflowName;
                        dbWorkflow.IsActive = apiWorkflow.IsActive;
                        dbWorkflow.MultiExecBehavior = apiWorkflow.MultiExecBehavior;
                        await workflowRepository.UpdateAsync(dbWorkflow);
                    }
                }

                // Remove workflows not in API
                var apiWorkflowIds = workflowsFromApi.Select(w => w.WorkflowId).ToHashSet();
                foreach (var dbWorkflow in workflowsFromDb)
                {
                    if (!apiWorkflowIds.Contains(dbWorkflow.WorkflowId))
                    {
                        await workflowRepository.DeleteAsync(dbWorkflow.WorkflowId);
                    }
                }

                await workflowRepository.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }


    }


}
