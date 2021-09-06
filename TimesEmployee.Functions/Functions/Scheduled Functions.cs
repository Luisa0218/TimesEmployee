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

            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("times", Connection = "AzureWebJobsStorage")] CloudTable timesTable,
            [Table("timesconsolidate", Connection = "AzureWebJobsStorage")] CloudTable timesTable2,
            ILogger log)

        {

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidate", QueryComparisons.Equal, false);
            TableQuery<TimesEntity> query = new TableQuery<TimesEntity>().Where(filter);
            TableQuerySegment<TimesEntity> allTimesEntity = await timesTable.ExecuteQuerySegmentedAsync(query, null);

            TableQuery<ConsolidatedEntity> queryconsolidatedtable = new TableQuery<ConsolidatedEntity>().Where(filter);
            TableQuerySegment<ConsolidatedEntity> allTimesConsolidated = await timesTable2.ExecuteQuerySegmentedAsync(queryconsolidatedtable, null);


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
                            await CreatedConsolidate(allTimesConsolidated, reiterate, reiteratetwo, dateCalculated, timesTable2);
                        }
                    }
                }
            }

        }
        public static async Task CreatedConsolidate(TableQuerySegment<ConsolidatedEntity> consolidatedEntity, TimesEntity dateTable, TimesEntity dateTableTwo, TimeSpan dateCalculated, CloudTable timesTable2)
        {
            if (consolidatedEntity.Results.Count == 0)
            {
                ConsolidatedEntity TimesConsolidate = new ConsolidatedEntity
                {
                    IdEmployee = dateTable.IdEmployee,
                    Date = dateTable.DateHour,
                    Minutes = dateCalculated.Minutes,
                    PartitionKey = "TIMES",
                    RowKey = Guid.NewGuid().ToString(),
                    ETag = "*"
                };

                TableOperation insertTimesConsolidate = TableOperation.Insert(TimesConsolidate);
                await timesTable2.ExecuteAsync(insertTimesConsolidate);
            }
            else
            {
                foreach (ConsolidatedEntity reiterateconsolidated in consolidatedEntity)
                {
                    //log.LogInformation("Actualizando consolidado segunda tabla");
                    if (reiterateconsolidated.IdEmployee == dateTable.IdEmployee)
                    {

                        ConsolidatedEntity TimesConsolidate = new ConsolidatedEntity
                        {
                            IdEmployee = reiterateconsolidated.IdEmployee,
                            Date = reiterateconsolidated.Date,
                            Minutes = (double)(reiterateconsolidated.Minutes + dateCalculated.TotalMinutes),
                            PartitionKey = reiterateconsolidated.PartitionKey,
                            RowKey = reiterateconsolidated.RowKey,
                            ETag = "*"
                        };

                        TableOperation insertConsolidate = TableOperation.Replace(TimesConsolidate);
                        await timesTable2.ExecuteAsync(insertConsolidate);
                    }
                    else
                    {
                        ConsolidatedEntity TimesConsolidateTwo = new ConsolidatedEntity
                        {
                            IdEmployee = dateTable.IdEmployee,
                            Date = dateTable.DateHour,
                            Minutes = dateCalculated.Minutes,
                            PartitionKey = "TIMES",
                            RowKey = Guid.NewGuid().ToString(),
                            ETag = "*"
                        };

                        TableOperation insertConsolidate = TableOperation.Insert(TimesConsolidateTwo);
                        await timesTable2.ExecuteAsync(insertConsolidate);
                    }
                }
            }

        }
    }
}
