using System;

namespace TimesEmployee.Common.Models
{
    public class Times
    {
        public int IdEmployee { get; set; }

        public DateTime DateHour { get; set; }

        public int Type { get; set; }

        public bool Consolidate { get; set; }

    }
}