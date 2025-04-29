using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GraphApiAudit.Console.Data;
using GraphApiAudit.Console.Services;
using Microsoft.EntityFrameworkCore;

namespace GraphApiAudit.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var metricsCollector = host.Services.GetRequiredService<GraphApiMetricsCollector>();
            await metricsCollector.CollectMetricsAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<GraphApiMetricsCollector>();
                    services.AddDbContext<AuditDbContext>(options =>
                        options.UseNpgsql(context.Configuration.GetConnectionString("PostgreSQL")));
                });
    }
}