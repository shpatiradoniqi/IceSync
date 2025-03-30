using IceSync.Server.Application.Services;
using IceSync.Server.Domain.Entities;
using IceSync.Server.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IceSync.Server.Controllers
{
    using IceSync.Server.Infrastructure.Persistence;
    using IceSync.Server.Infrastructure.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    namespace YourNamespace.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class WorkflowController : ControllerBase
        {
            private readonly IWorkflowRepository _workflowRepository;
            private readonly UniversalLoaderService _loaderService;
          

            public WorkflowController(IWorkflowRepository workflowRepository, UniversalLoaderService loaderService)
            {
                _workflowRepository = workflowRepository;
                _loaderService = loaderService;
               
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Workflow>>> GetWorkflows()
            {
                var workflows = await _workflowRepository.GetAllAsync();
                return Ok(workflows);
            }

            [HttpPost("{workflowId}/run")]
            public async Task<IActionResult> RunWorkflow(int workflowId)
            {
                var success = await _loaderService.RunWorkflowAsync(workflowId);
                if (success)
                {
                    return Ok(new { Message = "Workflow executed successfully." });
                }
                return BadRequest(new { Message = "Failed to execute workflow." });
            }


            //I have done this for testing manually and for my own understanding of the code,do not include it during the assessment

            [HttpPost("sync")]
            public async Task<IActionResult> SyncWorkflows()
            {
                // Fetch workflows from the API as DTOs
                var workflowsFromApiDto = await _loaderService.GetWorkflowsAsync();
                var workflowsFromDb = (await _workflowRepository.GetAllAsync()).ToList();


                // Map DTOs to entity models  
                var workflowsFromApi = workflowsFromApiDto.Select(dto => new Workflow
                {
                    WorkflowId = dto.Id,
                    WorkflowName = string.IsNullOrEmpty(dto.Name) ? "Unknown" : dto.Name,
                    IsActive = dto.IsActive,
                    MultiExecBehavior = dto.MultiExecBehavior
                }).ToList();

                // Sync API workflows with the database  
                foreach (var apiWorkflow in workflowsFromApi)
                {
                    var dbWorkflow = workflowsFromDb.FirstOrDefault(w => w.WorkflowId == apiWorkflow.WorkflowId);

                    if (dbWorkflow == null)
                    {
                        // Insert new workflows if they don’t exist  
                        await _workflowRepository.AddAsync(apiWorkflow);
                    }
                    else
                    {
                        // Update existing workflows with API data  
                        dbWorkflow.WorkflowName = apiWorkflow.WorkflowName;
                        dbWorkflow.IsActive = apiWorkflow.IsActive;
                        dbWorkflow.MultiExecBehavior = apiWorkflow.MultiExecBehavior;
                        await _workflowRepository.UpdateAsync(dbWorkflow);
                    }
                }

                // Remove any workflows in the DB that aren’t in the API  
                var apiWorkflowIds = workflowsFromApi.Select(w => w.WorkflowId).ToHashSet();
                foreach (var dbWorkflow in workflowsFromDb)
                {
                    if (!apiWorkflowIds.Contains(dbWorkflow.WorkflowId))
                    {

                        await _workflowRepository.DeleteAsync(dbWorkflow.WorkflowId);
                    }
                }


                await _workflowRepository.SaveChangesAsync();

                return Ok(new { Message = "Workflows synchronized successfully." });
            }

        }

    }
}