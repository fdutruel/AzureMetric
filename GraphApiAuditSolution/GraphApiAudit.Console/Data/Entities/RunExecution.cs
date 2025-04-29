using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class RunExecution
    {
        public int Id { get; set; }
        public DateTime ExecutedAt { get; set; }
        public long DurationMs { get; set; }
        public string Status { get; set; }
    }
}