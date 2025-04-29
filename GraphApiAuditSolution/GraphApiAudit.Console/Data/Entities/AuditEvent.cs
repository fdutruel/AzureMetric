using System;

namespace GraphApiAudit.Console.Data.Entities
{
    public class AuditEvent
    {
        public int Id { get; set; }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public string Actor { get; set; }
        public string ChangeType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}