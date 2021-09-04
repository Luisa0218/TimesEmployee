using System;

namespace TimesEmployee.Common.Responses
{
    public class Response
    {
        public int IdEmployee { get; set; }

        public DateTime DateHour { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}

