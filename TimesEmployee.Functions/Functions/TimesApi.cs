using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TimesEmployee.Common.Models;
using TimesEmployee.Common.Responses;
using TimesEmployee.Functions.Entities;

namespace TimesEmployee.Functions.Functions
{
    public static class TimesApi
    {
        [FunctionName(nameof(CreatedTimes))]
        public static async Task<IActionResult> CreatedTimes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "times")] HttpRequest req,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            ILogger log)
        {
            log.LogInformation("Received a new Employee");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Times times = JsonConvert.DeserializeObject<Times>(requestBody);

            if (string.IsNullOrEmpty(times?.IdEmployee.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {

                    Message = "The request must have a employee id."

                });
            }

            TimesEntity timesEntity = new TimesEntity
            {
                IdEmployee = times.IdEmployee,
                DateHour = DateTime.UtcNow,
                Type = times.Type,
                Consolidate = false,
                PartitionKey = "TIMES",
                RowKey = Guid.NewGuid().ToString(),
                ETag = "*"

            };

            TableOperation addOperation = TableOperation.Insert(timesEntity);
            await timesTable.ExecuteAsync(addOperation);

            string message = "new employee store in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                IdEmployee = times.IdEmployee,
                DateHour = DateTime.UtcNow,
                Message = message,
                Result = timesEntity


            });


        }

        [FunctionName(nameof(UpdateTimes))]
        public static async Task<IActionResult> UpdateTimes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "times/{Id}")] HttpRequest req,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            string Id,
            ILogger log)
        {
            log.LogInformation($"Update for IdEmployee: {Id}, in timesTable.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Times times = JsonConvert.DeserializeObject<Times>(requestBody);

            // Validate Employee Id 
            TableOperation findOperation = TableOperation.Retrieve<TimesEntity>("TIMES", Id);
            TableResult findResult = await timesTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)

            {
                return new BadRequestObjectResult(new Response
                {

                    Message = " id employee not found ."

                });
            }

            //Update Employee id 

            TimesEntity timesEntity = (TimesEntity)findResult.Result;
            timesEntity.DateHour = times.DateHour;
            timesEntity.Type = times.Type;

            if (!string.IsNullOrEmpty(times.IdEmployee.ToString()))
            {
                timesEntity.Type = times.Type;
            }

            TableOperation addOperation = TableOperation.Replace(timesEntity);
            await timesTable.ExecuteAsync(addOperation);

            string message = $"Update: {Id} update in timesTable.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                IdEmployee = times.IdEmployee,
                Message = message,
                Result = timesEntity


            });




        }

        [FunctionName(nameof(GetAllTimes))]
        public static async Task<IActionResult> GetAllTimes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "times")] HttpRequest req,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            ILogger log)
        {
            log.LogInformation("Get all timesEmployee received.");

            TableQuery<TimesEntity> query = new TableQuery<TimesEntity>();
            TableQuerySegment<TimesEntity> times = await timesTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all timesEmployee.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                Message = message,
                Result = times

            });
        }

        [FunctionName(nameof(GetTimesById))]
        public static IActionResult GetTimesById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "times/{Id}")] HttpRequest req,
            [Table("times", "TIMES", "{Id}", Connection = "AzureWebJobsStorage")] TimesEntity timesEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get todo by idEmployee: {id}, received.");

            if (timesEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {

                    Message = "TimesEmployee not found."
                });
            }

            string message = $"TimesEmployee: {timesEntity.RowKey}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                Message = message,
                Result = timesEntity
            });
        }

        [FunctionName(nameof(DeleteTimes))]
        public static async Task<IActionResult> DeleteTimes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "times/{Id}")] HttpRequest req,
            [Table("times", "TIMES", "{Id}", Connection = "AzureWebJobsStorage")] TimesEntity timesEntity,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete timesEmployee: {id}, received.");

            if (timesEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {

                    Message = "TimesEmployee not found."
                });
            }

            await timesTable.ExecuteAsync(TableOperation.Delete(timesEntity));
            string message = $"TimesEmployee: {timesEntity.RowKey}, deleted.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {

                Message = message,
                Result = timesEntity
            });
        }


    }

}