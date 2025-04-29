using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using GraphApiAudit.Console.Data;
using GraphApiAudit.Console.Data.Entities;

namespace GraphApiAudit.Console.Services
{
    public class GraphApiMetricsCollector
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AuditDbContext _dbContext;

        public GraphApiMetricsCollector(IConfiguration configuration, AuditDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpClient = new HttpClient();
            Authenticate();
        }

        private void Authenticate()
        {
            var tenantId = _configuration["GraphApi:TenantId"];
            var clientId = _configuration["GraphApi:ClientId"];
            var certificateThumbprint = _configuration["GraphApi:CertificateThumbprint"];
            var certificateStore = _configuration["GraphApi:CertificateStore"];

            var certificate = GetCertificate(certificateThumbprint, certificateStore);
            var token = GetAccessToken(tenantId, clientId, certificate).Result;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private X509Certificate2 GetCertificate(string thumbprint, string storeLocation)
        {
            var store = new X509Store(storeLocation == "CurrentUser" ? StoreName.My : StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            if (certs.Count > 0)
            {
                return certs[0];
            }
            throw new Exception("Certificate not found.");
        }

        private async Task<string> GetAccessToken(string tenantId, string clientId, X509Certificate2 certificate)
        {
            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(authority)
                .WithCertificate(certificate)
                .Build();
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        private async Task CollectApiCallMetrics()
        {
            var url = $"{_configuration["GraphApi:BaseUrl"]}/reports/getOffice365ActiveUserDetail(period='D7')";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;
            var content = await response.Content.ReadAsStringAsync();
            // Exemple de parsing CSV retourné par GraphAPI (getOffice365ActiveUserDetail retourne du CSV)
            var metrics = new List<ApiCallMetric>();
            var lines = content.Split('\n');
            if (lines.Length > 1)
            {
                // Première ligne = header, on commence à 1
                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = lines[i].Split(',');
                    if (cols.Length > 5)
                    {
                        metrics.Add(new ApiCallMetric
                        {
                            Endpoint = "/reports/getOffice365ActiveUserDetail",
                            CallCount = 1, // Pas de volume direct, à adapter selon vos besoins
                            AvgDuration = 0,
                            Date = DateTime.TryParse(cols[0], out var d) ? d : DateTime.UtcNow
                        });
                    }
                }
            }
            if (metrics.Count > 0)
            {
                _dbContext.ApiCallMetrics.AddRange(metrics);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CollectWorkloadUsageStats()
        {
            var url = $"{_configuration["GraphApi:BaseUrl"]}/reports/getOffice365ServicesUserCounts(period='D7')";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;
            var content = await response.Content.ReadAsStringAsync();
            // getOffice365ServicesUserCounts retourne aussi du CSV
            var stats = new List<WorkloadUsageStat>();
            var lines = content.Split('\n');
            if (lines.Length > 1)
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = lines[i].Split(',');
                    if (cols.Length > 2)
                    {
                        stats.Add(new WorkloadUsageStat
                        {
                            WorkloadType = cols[0],
                            UserId = "-",
                            UsageDate = DateTime.UtcNow // Adapter si la date est présente
                        });
                    }
                }
            }
            if (stats.Count > 0)
            {
                _dbContext.WorkloadUsageStats.AddRange(stats);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CollectAuditEvents()
        {
            // Limite à 1 jour, pagination, select
            var startDate = DateTime.UtcNow.AddDays(-1).ToString("o");
            var endDate = DateTime.UtcNow.ToString("o");
            var url = $"{_configuration["GraphApi:BaseUrl"]}/auditLogs/directoryAudits?$filter=activityDateTime ge {startDate} and activityDateTime le {endDate}&$top=100&$select=activityDateTime,activityDisplayName,initiatedBy,targetResources";
            var events = new List<AuditEvent>();
            while (!string.IsNullOrEmpty(url))
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) break;
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);
                if (json != null && json.value != null)
                {
                    foreach (var item in json.value)
                    {
                        events.Add(new AuditEvent
                        {
                            ObjectType = item.targetResources != null && item.targetResources.Count > 0 ? (string)item.targetResources[0].type : "",
                            ObjectId = item.targetResources != null && item.targetResources.Count > 0 ? (string)item.targetResources[0].id : "",
                            Actor = item.initiatedBy != null && item.initiatedBy.user != null ? (string)item.initiatedBy.user.displayName : "",
                            ChangeType = item.activityDisplayName != null ? (string)item.activityDisplayName : "",
                            Timestamp = item.activityDateTime != null ? (DateTime)item.activityDateTime : DateTime.UtcNow
                        });
                    }
                }
                url = json["@odata.nextLink"] != null ? (string)json["@odata.nextLink"] : null;
            }
            if (events.Count > 0)
            {
                _dbContext.AuditEvents.AddRange(events);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CollectApiConnectorUsage()
        {
            var url = $"{_configuration["GraphApi:BaseUrl"]}/identity/apiConnectors";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;
            var content = await response.Content.ReadAsStringAsync();
            // Parsing JSON pour apiConnectors
            dynamic json = JsonConvert.DeserializeObject(content);
            var connectors = new List<ApiConnectorUsage>();
            if (json != null && json.value != null)
            {
                foreach (var item in json.value)
                {
                    connectors.Add(new ApiConnectorUsage
                    {
                        Name = item.displayName != null ? (string)item.displayName : "",
                        CallCount = 0, // À adapter si info disponible
                        LastCallDate = DateTime.UtcNow, // À adapter si info disponible
                        Status = item.state != null ? (string)item.state : ""
                    });
                }
            }
            if (connectors.Count > 0)
            {
                _dbContext.ApiConnectorUsages.AddRange(connectors);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CollectAppRegistrationsAndUsage()
        {
            // Récupère toutes les applications enregistrées (pagination, $select)
            var url = $"{_configuration["GraphApi:BaseUrl"]}/applications?$select=id,displayName,appId&$top=100";
            var usages = new List<AppRegistrationUsage>();
            while (!string.IsNullOrEmpty(url))
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) break;
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);
                if (json != null && json.value != null)
                {
                    foreach (var app in json.value)
                    {
                        string appId = app.appId != null ? (string)app.appId : null;
                        string displayName = app.displayName != null ? (string)app.displayName : null;
                        int apiCallCount = 0;
                        if (!string.IsNullOrEmpty(appId))
                        {
                            // Pour chaque app, compter les appels dans les logs d'audit (signIns) sur 1 jour, paginé, $select
                            var startDate = DateTime.UtcNow.AddDays(-1).ToString("o");
                            var endDate = DateTime.UtcNow.ToString("o");
                            var signInUrl = $"{_configuration["GraphApi:BaseUrl"]}/auditLogs/signIns?$filter=appId eq '{appId}' and createdDateTime ge {startDate} and createdDateTime le {endDate}&$top=100&$select=appId";
                            while (!string.IsNullOrEmpty(signInUrl))
                            {
                                var signInResponse = await _httpClient.GetAsync(signInUrl);
                                if (!signInResponse.IsSuccessStatusCode) break;
                                var signInContent = await signInResponse.Content.ReadAsStringAsync();
                                dynamic signInJson = JsonConvert.DeserializeObject(signInContent);
                                if (signInJson != null && signInJson.value != null)
                                {
                                    foreach (var _ in signInJson.value)
                                    {
                                        apiCallCount++;
                                    }
                                }
                                signInUrl = signInJson["@odata.nextLink"] != null ? (string)signInJson["@odata.nextLink"] : null;
                            }
                        }
                        usages.Add(new AppRegistrationUsage
                        {
                            AppId = appId,
                            DisplayName = displayName,
                            ApiCallCount = apiCallCount,
                            Date = DateTime.UtcNow
                        });
                    }
                }
                url = json["@odata.nextLink"] != null ? (string)json["@odata.nextLink"] : null;
            }
            if (usages.Count > 0)
            {
                _dbContext.AppRegistrationUsages.AddRange(usages);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CollectMetricsAsync()
        {
            var run = new RunExecution
            {
                ExecutedAt = DateTime.UtcNow,
                Status = "Started"
            };
            _dbContext.RunExecutions.Add(run);
            await _dbContext.SaveChangesAsync();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await CollectApiCallMetrics();
                await CollectWorkloadUsageStats();
                await CollectAuditEvents();
                await CollectApiConnectorUsage();
                await CollectAppRegistrationsAndUsage();
                await CollectAppRegistrationsAndUsage();
                run.Status = "Success";
            }
            catch
            {
                run.Status = "Failed";
                throw;
            }
            finally
            {
                sw.Stop();
                run.DurationMs = (int)sw.ElapsedMilliseconds;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CollectAdvancedQuotaMetricsAsync()
        {
            // Limite à 1 jour, pagination, $select
            var startDate = DateTime.UtcNow.AddDays(-1).ToString("o");
            var endDate = DateTime.UtcNow.ToString("o");
            var url = $"{_configuration["GraphApi:BaseUrl"]}/auditLogs/signIns?$filter=createdDateTime ge {startDate} and createdDateTime le {endDate}&$top=100&$select=userId,appId,resourceDisplayName,createdDateTime,processingTimeInMilliseconds,status";
            var userApiMetrics = new List<UserApiCallMetric>();
            var appEndpointMetrics = new List<AppEndpointApiCallMetric>();
            var userAppMetrics = new List<UserAppApiCallMetric>();
            var errorMetrics = new List<ApiCallErrorMetric>();
            var throttlingMetrics = new List<ApiThrottlingMetric>();
            while (!string.IsNullOrEmpty(url))
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) break;
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);
                if (json != null && json.value != null)
                {
                    foreach (var item in json.value)
                    {
                        string userId = item.userId != null ? (string)item.userId : null;
                        string appId = item.appId != null ? (string)item.appId : null;
                        string endpoint = item.resourceDisplayName != null ? (string)item.resourceDisplayName : null;
                        DateTime date = item.createdDateTime != null ? (DateTime)item.createdDateTime : DateTime.UtcNow;
                        double duration = item.processingTimeInMilliseconds != null ? (double)item.processingTimeInMilliseconds : 0;
                        int statusCode = item.status != null && item.status.errorCode != null ? (int)item.status.errorCode : 0;
                        string errorMsg = item.status != null && item.status.failureReason != null ? (string)item.status.failureReason : null;
                        // UserApiCallMetric
                        userApiMetrics.Add(new UserApiCallMetric { UserId = userId, Endpoint = endpoint, CallCount = 1, AvgDuration = duration, Date = date });
                        // AppEndpointApiCallMetric
                        appEndpointMetrics.Add(new AppEndpointApiCallMetric { AppId = appId, Endpoint = endpoint, CallCount = 1, AvgDuration = duration, Date = date });
                        // UserAppApiCallMetric
                        userAppMetrics.Add(new UserAppApiCallMetric { UserId = userId, AppId = appId, Endpoint = endpoint, CallCount = 1, AvgDuration = duration, Date = date });
                        // ApiCallErrorMetric
                        if (statusCode >= 400)
                        {
                            errorMetrics.Add(new ApiCallErrorMetric { AppId = appId, UserId = userId, Endpoint = endpoint, StatusCode = statusCode, ErrorMessage = errorMsg, Date = date });
                        }
                        // ApiThrottlingMetric
                        if (statusCode == 429)
                        {
                            throttlingMetrics.Add(new ApiThrottlingMetric { AppId = appId, UserId = userId, Endpoint = endpoint, Date = date, ThrottleType = "429", Message = errorMsg });
                        }
                    }
                }
                url = json["@odata.nextLink"] != null ? (string)json["@odata.nextLink"] : null;
            }
            if (userApiMetrics.Count > 0) _dbContext.UserApiCallMetrics.AddRange(userApiMetrics);
            if (appEndpointMetrics.Count > 0) _dbContext.AppEndpointApiCallMetrics.AddRange(appEndpointMetrics);
            if (userAppMetrics.Count > 0) _dbContext.UserAppApiCallMetrics.AddRange(userAppMetrics);
            if (errorMetrics.Count > 0) _dbContext.ApiCallErrorMetrics.AddRange(errorMetrics);
            if (throttlingMetrics.Count > 0) _dbContext.ApiThrottlingMetrics.AddRange(throttlingMetrics);
            await _dbContext.SaveChangesAsync();
        }
    }
}