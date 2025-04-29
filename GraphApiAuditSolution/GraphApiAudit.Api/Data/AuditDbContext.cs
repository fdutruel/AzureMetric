using Microsoft.EntityFrameworkCore;

namespace GraphApiAudit.Api.Data
{
    /// <summary>
    /// Contexte de base de données pour l'audit GraphAPI et la gestion des métriques/quota.
    /// </summary>
    public class AuditDbContext : DbContext
    {
        /// <summary>
        /// Initialise une nouvelle instance du contexte AuditDbContext.
        /// </summary>
        /// <param name="options">Options de configuration du contexte.</param>
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

        /// <summary>
        /// Métriques d'appels API Graph (volume, durée, endpoint, date).
        /// </summary>
        public DbSet<ApiCallMetric> ApiCallMetrics { get; set; }
        /// <summary>
        /// Statistiques d’utilisation par workload Microsoft 365.
        /// </summary>
        public DbSet<WorkloadUsageStat> WorkloadUsageStats { get; set; }
        /// <summary>
        /// Logs d’audit des modifications d’objets AD.
        /// </summary>
        public DbSet<Entities.AuditEvent> AuditEvents { get; set; }
        /// <summary>
        /// Usage des connecteurs API personnalisés.
        /// </summary>
        public DbSet<ApiConnectorUsage> ApiConnectorUsages { get; set; }
        /// <summary>
        /// Historique des exécutions de collecte.
        /// </summary>
        public DbSet<RunExecution> RunExecutions { get; set; }
        /// <summary>
        /// Consommation des applications enregistrées (App Registration).
        /// </summary>
        public DbSet<Entities.AppRegistrationUsage> AppRegistrationUsages { get; set; }
        /// <summary>
        /// Appels API par utilisateur.
        /// </summary>
        public DbSet<Entities.UserApiCallMetric> UserApiCallMetrics { get; set; }
        /// <summary>
        /// Appels API par application et endpoint.
        /// </summary>
        public DbSet<Entities.AppEndpointApiCallMetric> AppEndpointApiCallMetrics { get; set; }
        /// <summary>
        /// Appels API par utilisateur et application.
        /// </summary>
        public DbSet<Entities.UserAppApiCallMetric> UserAppApiCallMetrics { get; set; }
        /// <summary>
        /// Erreurs d’appels API (4xx/5xx).
        /// </summary>
        public DbSet<Entities.ApiCallErrorMetric> ApiCallErrorMetrics { get; set; }
        /// <summary>
        /// Cas de throttling (429) lors des appels API.
        /// </summary>
        public DbSet<Entities.ApiThrottlingMetric> ApiThrottlingMetrics { get; set; }

        /// <summary>
        /// Configure le mapping des entités vers les tables de la base de données.
        /// </summary>
        /// <param name="modelBuilder">Le générateur de modèle EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiCallMetric>().ToTable("ApiCallMetrics");
            modelBuilder.Entity<WorkloadUsageStat>().ToTable("WorkloadUsageStats");
            modelBuilder.Entity<Entities.AuditEvent>().ToTable("AuditEvents");
            modelBuilder.Entity<ApiConnectorUsage>().ToTable("ApiConnectorUsage");
            modelBuilder.Entity<RunExecution>().ToTable("RunExecutions");
            modelBuilder.Entity<Entities.AppRegistrationUsage>().ToTable("AppRegistrationUsages");
            modelBuilder.Entity<Entities.UserApiCallMetric>().ToTable("UserApiCallMetrics");
            modelBuilder.Entity<Entities.AppEndpointApiCallMetric>().ToTable("AppEndpointApiCallMetrics");
            modelBuilder.Entity<Entities.UserAppApiCallMetric>().ToTable("UserAppApiCallMetrics");
            modelBuilder.Entity<Entities.ApiCallErrorMetric>().ToTable("ApiCallErrorMetrics");
            modelBuilder.Entity<Entities.ApiThrottlingMetric>().ToTable("ApiThrottlingMetrics");
        }
    }
}