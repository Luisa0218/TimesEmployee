using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TimesEmployee.Common.Models;
using TimesEmployee.Functions.Entities;
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

        [Fact]
        public async void UpdatedTimes_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Times timesRequest = TestFactory.GetTimesRequest();
            Guid timesId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreatedHttpRequest(timesId,timesRequest);

            //Act
            IActionResult response = await TimesApi.UpdateTimes(request, mockTimes, timesId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void GetTimesById_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimesEntity timesEntity = TestFactory.GetTimesEntity();
            Guid timesId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreatedHttpRequest(timesId);


            //Act
            IActionResult response =  TimesApi.GetTimesById(request, timesEntity, timesId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}

    


