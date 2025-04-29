using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class ApiConnectorUsage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CallCount { get; set; }
        public DateTime LastCallDate { get; set; }
        public string Status { get; set; }
    }
}