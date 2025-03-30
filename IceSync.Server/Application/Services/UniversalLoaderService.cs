using IceSync.Server.Domain.Entities;
using IceSync.Server.Infrastructure.DTO;
using System;
using System.Net.Http.Headers;

public class UniversalLoaderService
{
    private readonly IConfiguration _configuration;
    private string _token;
    private DateTime _tokenExpiry;

    public UniversalLoaderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

   
    public async Task<string?> GetTokenAsync()
    {
        using var client = new HttpClient();

        // like Swagger
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));



        var requestBody = new
        {
            apiCompanyId= _configuration["UniversalLoader:apiCompanyId"],
            apiUserId= _configuration["UniversalLoader:apiUserId"],
            apiUserSecret= _configuration["UniversalLoader:apiUserSecret"]
        };

        try
        {
           
            var response = await client.PostAsJsonAsync("https://api-test.universal-loader.com/authenticate", requestBody);

            
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token: {token}"); // I have done this for debugg
                return token;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }


    public class TokenResponse
    {
        public string Token { get; set; }
    }


   
    public async Task<IEnumerable<WorkflowDto>> GetWorkflowsAsync()
    {
        var token = await GetTokenAsync();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine($"Token: {token}");

        var response = await client.GetAsync($"{_configuration["UniversalLoader:BaseUrl"]}/workflows");
        response.EnsureSuccessStatusCode();

        // Deserialize into DTO first
        var workflowDtos = await response.Content.ReadFromJsonAsync<IEnumerable<WorkflowDto>>();

        // Map DTO to Entity
        var workflows = workflowDtos?.Select(dto => new Workflow
        {
            WorkflowId = dto.Id,  
            WorkflowName = dto.Name,  
            IsActive = dto.IsActive,
            MultiExecBehavior = dto.MultiExecBehavior
        }).ToList();


        

        return workflowDtos ?? Enumerable.Empty<WorkflowDto>();




    }



    public async Task<bool> RunWorkflowAsync(int workflowId)
    {
        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Failed to retrieve a valid token.");
            return false;
        }

        using var client = new HttpClient();

       
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);     
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        
        var url = $"{_configuration["UniversalLoader:BaseUrl"]}/workflows/{workflowId}/run";
        Console.WriteLine($"📡 Making API call to URL: {url}");

       
        var requestBody = new { }; // If api will need something extra

        try
        {
           
            var response = await client.PostAsync(
            $"{_configuration["UniversalLoader:BaseUrl"]}/workflows/{workflowId}/run",
            null);  

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Error in RunWorkflowAsync: {ex.Message}");
            return false;
        }
    }

}