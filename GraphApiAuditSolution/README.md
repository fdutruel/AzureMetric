# Graph API Audit Solution

This solution consists of two projects: a console application for collecting metrics from Microsoft Graph API and an ASP.NET Core REST API for exposing these metrics to a Vue.js frontend.

## Project Structure

```
GraphApiAuditSolution
├── README.md
├── GraphApiAudit.Console
│   ├── Program.cs
│   ├── Services
│   │   └── GraphApiMetricsCollector.cs
│   ├── Data
│   │   ├── AuditDbContext.cs
│   │   └── Entities
│   │       ├── ApiCallMetric.cs
│   │       ├── WorkloadUsageStat.cs
│   │       ├── AuditEvent.cs
│   │       ├── ApiConnectorUsage.cs
│   │       └── RunExecution.cs
│   ├── appsettings.json
│   └── GraphApiAudit.Console.csproj
├── GraphApiAudit.Api
│   ├── Controllers
│   │   ├── AuditController.cs
│   │   └── RunController.cs
│   ├── Data
│   │   ├── AuditDbContext.cs
│   │   └── Entities
│   │       ├── ApiCallMetric.cs
│   │       ├── WorkloadUsageStat.cs
│   │       ├── AuditEvent.cs
│   │       ├── ApiConnectorUsage.cs
│   │       └── RunExecution.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── appsettings.json
│   ├── Program.cs
│   ├── Startup.cs
│   ├── GraphApiAudit.Api.csproj
│   └── GraphApiAudit.Api.xml
├── GraphApiAuditSolution.sln
```

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- PostgreSQL Database
- Microsoft Azure account for Graph API access

### Configuration

1. **Database Setup**: Create a PostgreSQL database named `AuditDb` and configure the connection string in `appsettings.json` for both projects.

2. **Graph API Setup**:
   - Register an application in Azure Active Directory.
   - Obtain the Tenant ID, Client ID, and Certificate Thumbprint.
   - Configure the required permissions for Microsoft Graph API.

3. **Update Configuration Files**:
   - Update the `appsettings.json` files in both projects with your Graph API and database connection details.

### Running the Projects

1. **Console Application**:
   - Navigate to the `GraphApiAudit.Console` directory.
   - Run the application to start collecting metrics:
     ```
     dotnet run
     ```

2. **API Application**:
   - Navigate to the `GraphApiAudit.Api` directory.
   - Run the API:
     ```
     dotnet run
     ```
   - Access the Swagger UI at `http://localhost:<port>/swagger` to interact with the API endpoints.

## Usage Guidelines

- Use the console application to collect metrics from Microsoft Graph API.
- Use the REST API to retrieve the collected metrics and trigger new data collection executions.
- Ensure that the necessary permissions are granted for the Graph API to function correctly.

## License

This project is licensed under the MIT License.