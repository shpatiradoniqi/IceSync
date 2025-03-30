# IceSync

IceSync - Universal Loader Sync App

Introduction

This is a .NET 8 Web App called IceSync, built to synchronize workflows from the Universal Loader API into a local SQL Server database. It also provides a UI built with Angular to display the workflows and allows users to manually trigger workflows.

Tech Stack

Backend: .NET 8 (Minimal Clean Architecture)

Frontend: Angular (for displaying the workflows grid and API communication)

Database: SQL Server

Architecture: Repository Pattern with folder separation (DOMAIN, APPLICATION, INFRASTRUCTURE, CONTROLLERS)

This architecture ensures modularity, testability, and adherence to separation of concerns

Implementation Details

Folder Structure

I followed a Clean Architecture-like approach but without full implementation, just to demonstrate familiarity with it.

Domain: Contains Workflow.cs entity and IWorkflowRepository interface.

Application: Implements the core logic and services.

UniversalLoaderService.cs handles API authentication and data fetching (GetToken(), GetWorkflowsAsync(), RunWorkflowAsync()).

WorkflowService.cs handles data synchronization, including inserting, updating, and deleting workflows based on API data.

Infrastructure: Handles persistence.

Persistence - Implements DbContext.

Repositories - Implements WorkflowRepository.

DTOs - Defines WorkflowDto.

Controllers:

WorkflowController.cs - Exposes GetWorkflows and RunWorkflow API endpoints.

Photo where it seems working this icesync also with button "RunWorkFlow" included:
![working grid](https://github.com/user-attachments/assets/02613b74-32da-4479-8c7f-a009beed887e)

