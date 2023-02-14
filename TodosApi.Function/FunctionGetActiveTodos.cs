using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;

namespace TodosApi.Function
{
    public class FunctionGetActiveTodos
    {
        private readonly HttpClient client = new() 
        {
            BaseAddress = new Uri("https://localhost:7007")
        };

        [FunctionName("FunctionGetActiveTodos")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Queue("activtodos",Connection = "storageaccountconn")] IAsyncCollector<string> collector,
            ILogger log)
        {
            
            var response = await client.GetAsync("api/todoitems/uncompleted");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TodoItemEntiTy[]>();
        
            await collector.AddAsync($"{result.Count()} uncompleted todos");

            return new OkObjectResult(result);
        }
    }
}
