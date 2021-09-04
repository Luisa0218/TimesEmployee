using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimesEmployee.Functions.Entities
{
    public class TimesEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime DateHour { get; set; }

        public int Type { get; set; }

        public bool Consolidate { get; set; }

    }
}
