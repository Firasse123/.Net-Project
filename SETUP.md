# Setup Guide for Local Development

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 LocalDB or Full Edition
- Visual Studio 2022 or VS Code

## Quick Start

### 1. Clone and Navigate
```bash
git clone <repository-url>
cd EmployeesManagement
```

### 2. Configure Database Connection

**Option A: Using User Secrets (Recommended)**
```bash
cd EmployeesManagement
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True"
```

**Option B: Create appsettings.json**
```bash
# Copy the example file
copy appsettings.example.json appsettings.json

# Edit appsettings.json and add your connection string
```

### 3. Apply Database Migrations
```bash
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run --urls "http://localhost:5055"
```

### 5. Access the App
Navigate to: http://localhost:5055

## Database Connection Strings Examples

### LocalDB
```
Server=(localdb)\mssqllocaldb;Database=EmployeeDB;Trusted_Connection=True;
```

### SQL Server Express (Local)
```
Server=localhost\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

### SQL Server with Authentication
```
Server=SERVER_NAME;Database=EmployeeDB;User Id=sa;Password=YOUR_PASSWORD;
```

## Environment Varieables

For development, these are optional (User Secrets recommended):
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://localhost:5055`

For production:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<your-prod-connection-string>`

## Troubleshooting

### "Database connection failed"
- Verify SQL Server is running
- Check connection string in `appsettings.json` or User Secrets
- Ensure database name exists

### "Migrations not applied"
```bash
dotnet ef database update --project EmployeesManagement/EmployeesManagement.csproj
```

### "Port 5055 already in use"
```bash
# Run on different port
dotnet run --urls "http://localhost:5056"
```

### "Build fails with NuGet errors"
```bash
dotnet clean
dotnet restore
dotnet build
```

## Creating a New Database

If you want to start fresh:

```bash
# Drop and recreate
dotnet ef database drop
dotnet ef database update
```

## Debugging

### Using Visual Studio
1. Open the .sln file
2. Set breakpoints in your code
3. Press F5 to start debugging

### Using VS Code
1. Install "C# Dev Kit" extension
2. Use the Debug tab to start debugging
3. Set breakpoints in your code

## Additional Resources

- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [User Secrets Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
