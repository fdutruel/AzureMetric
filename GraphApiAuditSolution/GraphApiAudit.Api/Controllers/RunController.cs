using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GraphApiAudit.Api.Controllers
{
    [ApiController]
    [Route("audit")]
    public class RunController : ControllerBase
    {
        private readonly IHost _host;

        public RunController(IHost host)
        {
            _host = host;
        }

        [HttpPost("run")]
        public IActionResult RunMetricsCollection()
        {
            // Start the console application for metrics collection
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "GraphApiAudit.Console.dll",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                return Ok(new { Status = "Metrics collection started." });
            }
        }
    }
}