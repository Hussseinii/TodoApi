using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TodosApi.Function;

namespace TodosApi.Functions
{
    public class AddUncompletedTodosToTableStorage
    {
        private readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://localhost:7007")
        };

        [FunctionName("AddUncompletedTodosToTableStorage")]
        public async Task Run([QueueTrigger("activtodos", Connection = "storageaccountconn")]string myQueueItem,
            [Table("ActivtodosTb", Connection = "storageaccountconn")] IAsyncCollector<TodoItemEntiTy> collector,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var response = await client.GetAsync("api/todoitems/uncompleted");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TodoItemEntiTy[]>();

            foreach(var todo in result)
            {
                todo.PartitionKey = "Todos";
                todo.RowKey = todo.Id.ToString();
                
                await collector.AddAsync(todo);

                log.LogInformation($"Added todo with id {todo.Id}");
            }
        }
    }
}
