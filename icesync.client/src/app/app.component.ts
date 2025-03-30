import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';




@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  workflows: any[] = [];
  message: string = '';
  private baseUrl = 'https://localhost:7257/api/workflow'; 

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    setTimeout(() => this.loadWorkflows(), 1000);
  }

  // Fetch all workflows from the API
  loadWorkflows(): void {
    this.http.get<any[]>(`${this.baseUrl}`).subscribe({
      next: (data) => {
        this.workflows = data;
        this.message = '';
      },
      error: (err) => {
        this.message = 'Failed to load workflows: ' + err.message;
        this.workflows = [];
      }
    });
  }

  // Run a specific workflow by its ID
  runWorkflow(workflowId: string): void {
    this.http.post(`${this.baseUrl}/${workflowId}/run`, {}).subscribe({
      next: () => {
        this.message = `Workflow ${workflowId} executed successfully.`;
      },
      error: (err) => {
        this.message = `Failed to execute workflow ${workflowId}: ` + err.message;
      }
    });
  }


}


