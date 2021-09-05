using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using TimesEmployee.Functions.Entities;

namespace TimesEmployee.Functions.Functions
{
    public static class Scheduled_Functions
    {
        [FunctionName(nameof(ProgrammerTimes))]
        public static async Task ProgrammerTimes(
            
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            ILogger log)
           
        {
            // TimesEntity = Tabla 1
            string filter = TableQuery.GenerateFilterConditionForBool("Consolidate", QueryComparisons.Equal, false);
            TableQuery<TimesEntity> query = new TableQuery<TimesEntity>().Where(filter);
            TableQuerySegment<TimesEntity> allTimesEntity = await timesTable.ExecuteQuerySegmentedAsync(query, null);

            //CheckConsolidateEntity = Tabla 2
            //TableQuery<ConsolidateEntity> queryConsolidate = new TableQuery<ConsolidateEntity>();
            //TableQuerySegment<ConsolidateEntity> allCheckConsolidateEntity = await workingTimeTable.ExecuteQuerySegmentedAsync(queryConsolidate, null);

            log.LogInformation($"First foreach");
            foreach (TimesEntity reiterate in allTimesEntity)
            {
                log.LogInformation($"First if");
                if (!string.IsNullOrEmpty(reiterate.IdEmployee.ToString()) && reiterate.Type == 0)
                {
                    log.LogInformation($"Second foreach");
                    foreach (TimesEntity reiteratetwo in allTimesEntity)
                    {
                        TimeSpan dateCalculated = (reiteratetwo.DateHour - reiterate.DateHour);
                        if (reiteratetwo.IdEmployee == reiterate.IdEmployee && reiteratetwo.Type == 1)
                        {
                           
                            TimesEntity Times = new TimesEntity
                            {
                                IdEmployee = reiteratetwo.IdEmployee,
                                DateHour = reiteratetwo.DateHour,
                                Type = reiteratetwo.Type,
                                Consolidate = true,
                                PartitionKey = "TIMES",
                                RowKey = reiteratetwo.RowKey,
                                ETag = "*"
                            };

                            TimesEntity TimesTwo = new TimesEntity
                            {
                                IdEmployee = reiterate.IdEmployee,
                                DateHour = reiterate.DateHour,
                                Type = reiterate.Type,
                                Consolidate = true,
                                PartitionKey = "TIMES",
                                RowKey = reiterate.RowKey,
                                ETag = "*"
                            };

                            TableOperation updateTimesEntity = TableOperation.Replace(Times);
                            await timesTable.ExecuteAsync(updateTimesEntity);

                            TableOperation updateTimesEntityTwo = TableOperation.Replace(TimesTwo);
                            await timesTable.ExecuteAsync(updateTimesEntityTwo);

                        }
                    }
                }
            }
        }
    }
}
