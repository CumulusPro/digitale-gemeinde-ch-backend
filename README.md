
# Form Designer Backend

This is the backend solution for the **Form Designer** application, implemented using ASP.NET Core. It provides APIs and shared components to support form management, storage, and integrations.

## 🧱 Project Structure

```
formdesignerbackend/
│
├── database/                          # SQL scripts for database setup
│   └── FormDesignerSQL Create script.sql
│
├── src/                               # Source code
│   ├── Cpro.Forms.Api/               # Main API project
│   ├── Cpro.Forms.Application/       # Application layer logic
│   ├── Cpro.Forms.Domain/            # Domain models and business rules
│   ├── Cpro.Forms.Infrastructure/    # Infrastructure services (e.g., persistence)
│   ├── Common/                       # Shared libraries and utilities
│   │   └── Peritos.Common.*         # Reusable common components
│   └── Tests/                        # Unit and integration tests
│
├── formdesignerapi.sln               # Solution file
└── azure-pipelines-1.yml             # Azure DevOps pipeline definition
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server
- Azure CLI (for deployments)
- Visual Studio or VS Code

### Running Locally

1. Clone the repository.
2. Set up the SQL database using the script in `database/FormDesignerSQL Create script.sql`.
3. Update connection strings and environment variables if needed.
4. Run the API:

```bash
cd src/Cpro.Forms.Api
dotnet run
```

### Tests

Run all unit tests:

```bash
dotnet test
```

## ⚙️ CI/CD

The project includes an Azure DevOps pipeline (`azure-pipelines-1.yml`) for continuous integration and deployment.

## 🧪 Test Coverage

Unit tests are located under:

- `Peritos.Common.Api.Tests`
- `Cpro.Forms.Application.Tests`
- `Cpro.Forms.Infrastructure.Tests`

## 📂 Database

- Located in `database/`
- Main script: `FormDesignerSQL Create script.sql`

## 🧰 Libraries and Patterns

- ASP.NET Core Web API
- Clean Architecture: Domain, Application, Infrastructure, API
- Application Insights integration
- Paging and Validation utilities

## 📄 License

Proprietary – Internal use only.
