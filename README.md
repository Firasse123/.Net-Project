# Employees Management System

A comprehensive ASP.NET Core 8.0 MVC application for managing employees, recruitment, compensation, equipment, and business workflows.

## 🎯 Features Implemented

### Phase 3: Recruitment Pipeline
- Candidate management with status tracking (Applied, Under Review, Interviewed, Offered, Hired, Rejected)
- Job openings management
- Interview scheduling
- Recruitment workflow validations

### Phase 4: Compensation Management
- Employee salary management
- Salary approval workflow (Pending → Approved/Rejected)
- Bulk salary updates
- Benefits management (Activate/Deactivate)
- Compensation reports and payroll summaries

### Phase 5: Equipment Tracking
- Equipment lifecycle management (Available → Assigned → Under Maintenance → Retired)
- Equipment assignment to employees
- Maintenance tracking
- Equipment audit reports

### Phase 6: Business Logic & Validations
- Hard-blocking recruitment workflow rules
- Model-level data annotations (StringLength, Range, Phone, Email)
- Controller-level validation
- Error handling and alerts

### Phase 7: Reporting Dashboard
- **Payroll Reports**: Total payroll, average salary, department breakdown, salary status summary
- **Equipment Audit**: Equipment by type, status breakdown, employee assignments
- **Recruitment Statistics**: Pipeline metrics, hire rates, position-wise candidate analysis

## 📋 Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core 8.0
- **Frontend**: Bootstrap 5 / AdminLTE 3.x
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download/dotnet)
- [SQL Server 2019 or later](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/EmployeesManagement.git
   cd EmployeesManagement
   ```

2. **Configure the database connection**
   - Copy `EmployeesManagement/appsettings.example.json` to understand the configuration structure
   - Create `EmployeesManagement/appsettings.json` with your connection string:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
       }
     }
     ```
   - **Alternative**: Use User Secrets for local development:
     ```bash
     cd EmployeesManagement
     dotnet user-secrets init
     dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
     ```

3. **Setup the database**
   ```bash
   cd EmployeesManagement
   dotnet ef database update
   ```

4. **Restore dependencies and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project EmployeesManagement/EmployeesManagement.csproj --urls "http://localhost:5055"
   ```

6. **Access the application**
   - Open your browser and navigate to `http://localhost:5055`
   - Register a new account or use the default credentials (if seeded)

## 📁 Project Structure

```
EmployeesManagement/
├── Controllers/              # MVC Controllers (Employees, Candidates, Salaries, etc.)
├── Models/                   # Domain models and ViewModels
├── Data/                      # Entity Framework DbContext and Migrations
├── Views/                     # Razor templates (HTML + C#)
│   ├── Employees/
│   ├── Candidates/
│   ├── Salaries/
│   ├── Equipment/
│   ├── Benefits/
│   ├── Shared/               # Layout and shared components
| └── Home/
├── wwwroot/                   # Static files (CSS, JS, images)
│   ├── css/
│   ├── js/
│   ├── lib/                   # NuGet packages (Bootstrap, jQuery, etc.)
│   └── AdminLTE/              # AdminLTE theme
├── appsettings.json           # ⚠️ DO NOT commit local connection strings
└── Program.cs                 # Startup configuration
```

## 🔧 Configuration

### Connection String Setup

**Option 1: Local SQL Server**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Option 2: SQL Server with Authentication**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=EmployeeDB;User Id=sa;Password=your-password;TrustServerCertificate=True"
  }
}
```

**Option 3: Azure SQL Database**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=EmployeeDB;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;"
  }
}
```

### User Secrets (Recommended for Development)

```bash
# Initialize secrets
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"

# List all secrets
dotnet user-secrets list
```

## 📊 Database Migrations

Create a new migration after model changes:
```bash
dotnet ef migrations add MigrationName --project EmployeesManagement/EmployeesManagement.csproj
dotnet ef database update
```

## 🧪 Testing

To test the application locally:

1. **Create test data**: Run the app and use the UI to add employees, candidates, etc.
2. **Navigate to different modules**: Use the sidebar menu to test Employees, Candidates, Salaries, Equipment, Benefits
3. **Check Reports**: Verify all three reports (Payroll Summary, Equipment Audit, Recruitment Stats) display correctly

## 🚢 Deployment

### Deploy to Azure App Service

1. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Create Azure resources** (if using Azure)
   - Azure App Service (Web App)
   - Azure SQL Database

3. **Configure connection string** in Azure App Service settings

4. **Deploy**
   ```bash
   # Using Azure CLI
   az webapp deployment source config-zip --resource-group myGroup --name myApp --src publish.zip
   ```

### Environment Variables

For production, set these environment variables:
- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ConnectionStrings__DefaultConnection`: Your production connection string
- `ASPNETCORE_URLS`: Your app URL

## 📝 API Endpoints (Key Controllers)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Employees` | GET | List all employees |
| `/Employees/Create` | POST | Add new employee |
| `/Candidates` | GET | List recruitment candidates |
| `/Salaries` | GET | View employee salaries |
| `/Salaries/Reports` | GET | Payroll report dashboard |
| `/Equipment` | GET | List equipment |
| `/Equipment/Audit` | GET | Equipment audit report |
| `/Candidates/Stats` | GET | Recruitment pipeline statistics |

## 🔐 Security Considerations

- ✅ User authentication using ASP.NET Core Identity
- ✅ Authorization checks on sensitive operations
- ✅ CSRF protection with anti-forgery tokens
- ⚠️ Do NOT commit `appsettings.json` with real connection strings
- ⚠️ Use User Secrets or environment variables for sensitive data
- 🔄 Regularly update NuGet dependencies for security patches

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit changes (`git commit -m 'Add YourFeature'`)
4. Push to branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📧 Support

For issues or questions:
- Open an issue on GitHub
- Check existing documentation
- Review the project structure for implementation details

## 🔄 Version History

- **v1.0.0** (Feb 2026): Initial release with Phases 1-7
  - Core employee management
  - Recruitment pipeline
  - Compensation management
  - Equipment tracking
  - Business logic validations
  - Comprehensive reporting

---

**Built with ❤️ using ASP.NET Core 8.0**
