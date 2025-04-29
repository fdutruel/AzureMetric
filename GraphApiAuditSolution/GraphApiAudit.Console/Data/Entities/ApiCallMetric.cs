using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class ApiCallMetric
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public int CallCount { get; set; }
        public double AvgDuration { get; set; }
        public DateTime Date { get; set; }
    }
}