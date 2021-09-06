using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using TimesEmployee.Common.Models;
using TimesEmployee.Functions.Entities;

namespace TimesEmployee.Test.Helpers
{
    public class TestFactory

    {
        public static TimesEntity GetTimesEntity()

        {
            return new TimesEntity
            {
                ETag = "*",
                PartitionKey = "TIMES",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = 1,
                DateHour = DateTime.UtcNow,
                Type = 0,
                Consolidate = false,


            };

        }

         public static DefaultHttpRequest  CreatedHttpRequest(Guid timesId, Times timesRequest)
         {
            string request = JsonConvert.SerializeObject(timesRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Body = GenerateStreamFromString(request),
                Path = $"/{timesId}"
            };

         }

        public static DefaultHttpRequest CreatedHttpRequest(Guid timesId)
        {
          
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Path = $"/{timesId}"
            };

        }

        public static DefaultHttpRequest CreatedHttpRequest(Times timesRequest)
        {
            string request = JsonConvert.SerializeObject(timesRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Body = GenerateStreamFromString(request)
                
            };

        }

        public static DefaultHttpRequest CreatedHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());

        }

        public static Times GetTimesRequest()
        {
            return new Times
            { 
                IdEmployee = 1,
                DateHour= DateTime.UtcNow,
                Type = 0,
                

            };
        }

        public static Stream GenerateStreamFromString(string StringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(StringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;

        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }

    }

}
