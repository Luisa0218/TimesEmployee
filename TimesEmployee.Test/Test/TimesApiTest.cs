using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TimesEmployee.Common.Models;
using TimesEmployee.Functions.Functions;
using TimesEmployee.Test.Helpers;
using Xunit;

namespace TimesEmployee.Test.Test
{
    public class TimesApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreatedTimes_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Times timesRequest = TestFactory.GetTimesRequest();
            DefaultHttpRequest request = TestFactory.CreatedHttpRequest(timesRequest);

            //Act
            IActionResult response = await TimesApi.CreatedTimes(request, mockTimes, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
