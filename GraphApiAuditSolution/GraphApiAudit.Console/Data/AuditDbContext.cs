using Microsoft.EntityFrameworkCore;

namespace GraphApiAudit.Console.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.ApiCallMetric> ApiCallMetrics { get; set; }
        public DbSet<Entities.WorkloadUsageStat> WorkloadUsageStats { get; set; }
        public DbSet<Entities.AuditEvent> AuditEvents { get; set; }
        public DbSet<Entities.ApiConnectorUsage> ApiConnectorUsages { get; set; }
        public DbSet<Entities.RunExecution> RunExecutions { get; set; }
        public DbSet<Entities.AppRegistrationUsage> AppRegistrationUsages { get; set; }
        public DbSet<Entities.UserApiCallMetric> UserApiCallMetrics { get; set; }
        public DbSet<Entities.AppEndpointApiCallMetric> AppEndpointApiCallMetrics { get; set; }
        public DbSet<Entities.UserAppApiCallMetric> UserAppApiCallMetrics { get; set; }
        public DbSet<Entities.ApiCallErrorMetric> ApiCallErrorMetrics { get; set; }
        public DbSet<Entities.ApiThrottlingMetric> ApiThrottlingMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.ApiCallMetric>().ToTable("ApiCallMetrics");
            modelBuilder.Entity<Entities.WorkloadUsageStat>().ToTable("WorkloadUsageStats");
            modelBuilder.Entity<Entities.AuditEvent>().ToTable("AuditEvents");
            modelBuilder.Entity<Entities.ApiConnectorUsage>().ToTable("ApiConnectorUsage");
            modelBuilder.Entity<Entities.RunExecution>().ToTable("RunExecutions");
            modelBuilder.Entity<Entities.AppRegistrationUsage>().ToTable("AppRegistrationUsages");
            modelBuilder.Entity<Entities.UserApiCallMetric>().ToTable("UserApiCallMetrics");
            modelBuilder.Entity<Entities.AppEndpointApiCallMetric>().ToTable("AppEndpointApiCallMetrics");
            modelBuilder.Entity<Entities.UserAppApiCallMetric>().ToTable("UserAppApiCallMetrics");
            modelBuilder.Entity<Entities.ApiCallErrorMetric>().ToTable("ApiCallErrorMetrics");
            modelBuilder.Entity<Entities.ApiThrottlingMetric>().ToTable("ApiThrottlingMetrics");
        }
    }
}