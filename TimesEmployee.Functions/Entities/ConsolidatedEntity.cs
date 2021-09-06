using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimesEmployee.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Date { get; set; }

        public double Minutes { get; set; }

      
    }
}