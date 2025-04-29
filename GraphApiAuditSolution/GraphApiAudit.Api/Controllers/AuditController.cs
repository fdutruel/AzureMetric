using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraphApiAudit.Api.Data;
using GraphApiAudit.Api.Data.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GraphApiAudit.Api.Controllers
{
    ///// <summary>
    ///// Controller pour gérer les appels API et les statistiques d'audit.
    ///// </summary>
    /// <remarks>
    /// Ce contrôleur fournit des endpoints pour récupérer les métriques d'appels API, les statistiques d'utilisation des workloads,
    /// les logs d'audit, et d'autres données liées à l'utilisation des API Microsoft Graph.
    /// </remarks>   
    [ApiController]
    [Route("audit")]    
    public class AuditController : ControllerBase
    {
        private readonly AuditDbContext _context;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur AuditController.
        /// </summary>
        public AuditController(AuditDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère la liste brute des appels API Graph collectés (volume, durée, endpoint, date).
        /// </summary>
        [HttpGet("api-calls")]
        public async Task<ActionResult<IEnumerable<ApiCallMetric>>> GetApiCallMetrics()
        {
            var metrics = await _context.ApiCallMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Récupère les statistiques d’utilisation par type de workload Microsoft 365 (Exchange, SharePoint, etc.).
        /// </summary>
        [HttpGet("workloads")]
        public async Task<ActionResult<IEnumerable<WorkloadUsageStat>>> GetWorkloadUsageStats()
        {
            var stats = await _context.WorkloadUsageStats.ToListAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Récupère les logs d’audit des modifications d’objets Active Directory.
        /// </summary>
        [HttpGet("audit-events")]
        public async Task<ActionResult<IEnumerable<AuditEvent>>> GetAuditEvents()
        {
            var events = await _context.AuditEvents.ToListAsync();
            return Ok(events);
        }

        /// <summary>
        /// Récupère les statistiques d’utilisation des connecteurs personnalisés.
        /// </summary>
        [HttpGet("api-connectors")]
        public async Task<ActionResult<IEnumerable<ApiConnectorUsage>>> GetApiConnectorUsage()
        {
            var usage = await _context.ApiConnectorUsages.ToListAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Récupère la consommation des applications enregistrées (App Registration).
        /// </summary>
        [HttpGet("app-registrations")]
        public async Task<ActionResult<IEnumerable<AppRegistrationUsage>>> GetAppRegistrationUsages()
        {
            var usages = await _context.AppRegistrationUsages.ToListAsync();
            return Ok(usages);
        }

        /// <summary>
        /// Récupère la liste brute des appels API par utilisateur.
        /// </summary>
        [HttpGet("user-api-calls")]
        public async Task<ActionResult<IEnumerable<UserApiCallMetric>>> GetUserApiCallMetrics()
        {
            var metrics = await _context.UserApiCallMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Récupère la liste brute des appels API par application et endpoint.
        /// </summary>
        [HttpGet("app-endpoint-api-calls")]
        public async Task<ActionResult<IEnumerable<AppEndpointApiCallMetric>>> GetAppEndpointApiCallMetrics()
        {
            var metrics = await _context.AppEndpointApiCallMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Récupère la liste brute des appels API par utilisateur et application.
        /// </summary>
        [HttpGet("user-app-api-calls")]
        public async Task<ActionResult<IEnumerable<UserAppApiCallMetric>>> GetUserAppApiCallMetrics()
        {
            var metrics = await _context.UserAppApiCallMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Récupère la liste brute des erreurs (4xx/5xx) lors des appels API.
        /// </summary>
        [HttpGet("api-call-errors")]
        public async Task<ActionResult<IEnumerable<ApiCallErrorMetric>>> GetApiCallErrorMetrics()
        {
            var metrics = await _context.ApiCallErrorMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Récupère la liste brute des cas de throttling (erreurs 429).
        /// </summary>
        [HttpGet("api-throttling")]
        public async Task<ActionResult<IEnumerable<ApiThrottlingMetric>>> GetApiThrottlingMetrics()
        {
            var metrics = await _context.ApiThrottlingMetrics.ToListAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Agrège le nombre total d’appels API par application sur la période (par défaut 7 jours).
        /// </summary>
        [HttpGet("app-registrations/aggregate-by-app")]
        public async Task<ActionResult<IEnumerable<object>>> GetAppRegistrationAggregates([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.AppRegistrationUsages
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.AppId)
                .Select(g => new {
                    AppId = g.Key,
                    DisplayName = g.Max(x => x.DisplayName),
                    TotalCalls = g.Sum(x => x.ApiCallCount)
                })
                .OrderByDescending(x => x.TotalCalls)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Agrège le nombre total d’appels API par utilisateur sur la période.
        /// </summary>
        [HttpGet("user-api-calls/aggregate-by-user")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserApiCallAggregates([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.UserApiCallMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.UserId)
                .Select(g => new {
                    UserId = g.Key,
                    TotalCalls = g.Sum(x => x.CallCount)
                })
                .OrderByDescending(x => x.TotalCalls)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retourne les endpoints les plus sollicités sur la période.
        /// </summary>
        [HttpGet("app-endpoint-api-calls/top-endpoints")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopEndpoints([FromQuery] int top = 5, [FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.AppEndpointApiCallMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.Endpoint)
                .Select(g => new {
                    Endpoint = g.Key,
                    TotalCalls = g.Sum(x => x.CallCount)
                })
                .OrderByDescending(x => x.TotalCalls)
                .Take(top)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Agrège le nombre total d’appels API par couple utilisateur/application.
        /// </summary>
        [HttpGet("user-app-api-calls/aggregate-by-user-app")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserAppApiCallAggregates([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.UserAppApiCallMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => new { x.UserId, x.AppId })
                .Select(g => new {
                    g.Key.UserId,
                    g.Key.AppId,
                    TotalCalls = g.Sum(x => x.CallCount)
                })
                .OrderByDescending(x => x.TotalCalls)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Agrège le nombre d’erreurs API par application.
        /// </summary>
        [HttpGet("api-call-errors/aggregate-by-app")]
        public async Task<ActionResult<IEnumerable<object>>> GetApiCallErrorsByApp([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.ApiCallErrorMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.AppId)
                .Select(g => new {
                    AppId = g.Key,
                    ErrorCount = g.Count()
                })
                .OrderByDescending(x => x.ErrorCount)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Agrège le nombre de cas de throttling (429) par application.
        /// </summary>
        [HttpGet("api-throttling/aggregate-by-app")]
        public async Task<ActionResult<IEnumerable<object>>> GetApiThrottlingByApp([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.ApiThrottlingMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.AppId)
                .Select(g => new {
                    AppId = g.Key,
                    ThrottleCount = g.Count()
                })
                .OrderByDescending(x => x.ThrottleCount)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retourne la moyenne d’appels par application et endpoint sur la période.
        /// </summary>
        [HttpGet("app-endpoint-api-calls/average-by-app-endpoint")]
        public async Task<ActionResult<IEnumerable<object>>> GetAverageByAppEndpoint([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.AppEndpointApiCallMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => new { x.AppId, x.Endpoint })
                .Select(g => new {
                    g.Key.AppId,
                    g.Key.Endpoint,
                    AvgCalls = g.Average(x => x.CallCount)
                })
                .OrderByDescending(x => x.AvgCalls)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Liste les utilisateurs ayant dépassé un certain seuil d’appels API sur la période.
        /// </summary>
        [HttpGet("user-api-calls/heavy-users")]
        public async Task<ActionResult<IEnumerable<object>>> GetHeavyUsers([FromQuery] int days = 7, [FromQuery] int threshold = 1000)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var result = await _context.UserApiCallMetrics
                .Where(x => x.Date >= startDate)
                .GroupBy(x => x.UserId)
                .Select(g => new {
                    UserId = g.Key,
                    TotalCalls = g.Sum(x => x.CallCount)
                })
                .Where(x => x.TotalCalls > threshold)
                .OrderByDescending(x => x.TotalCalls)
                .ToListAsync();
            return Ok(result);
        }
    }
}