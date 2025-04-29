using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class WorkloadUsageStat
    {
        public int Id { get; set; }
        public string WorkloadType { get; set; }
        public string UserId { get; set; }
        public DateTime UsageDate { get; set; }
    }
}