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

    }

}