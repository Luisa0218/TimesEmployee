using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimesEmployee.Test.Helpers
{
    public class MockCloudTableTimes : CloudTable
    {
        public MockCloudTableTimes(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableTimes(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri,
         credentials)
        {
        }

        public MockCloudTableTimes(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress,
        credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetTimesEntity()
            });
        }
    }

}
