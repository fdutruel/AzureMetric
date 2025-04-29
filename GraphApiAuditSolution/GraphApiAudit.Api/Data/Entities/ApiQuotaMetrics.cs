using System;

namespace GraphApiAudit.Api.Data.Entities
{
    public class UserApiCallMetric
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public int CallCount { get; set; }
        public double AvgDuration { get; set; }
        public DateTime Date { get; set; }
    }

    public class AppEndpointApiCallMetric
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string Endpoint { get; set; }
        public int CallCount { get; set; }
        public double AvgDuration { get; set; }
        public DateTime Date { get; set; }
    }

    public class UserAppApiCallMetric
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AppId { get; set; }
        public string Endpoint { get; set; }
        public int CallCount { get; set; }
        public double AvgDuration { get; set; }
        public DateTime Date { get; set; }
    }

    public class ApiCallErrorMetric
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }

    public class ApiThrottlingMetric
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public DateTime Date { get; set; }
        public string ThrottleType { get; set; }
        public string Message { get; set; }
    }
}
