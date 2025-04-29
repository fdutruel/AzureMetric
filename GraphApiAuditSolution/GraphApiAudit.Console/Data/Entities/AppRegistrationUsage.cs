using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class AppRegistrationUsage
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string DisplayName { get; set; }
        public int ApiCallCount { get; set; }
        public DateTime Date { get; set; }
    }
}
