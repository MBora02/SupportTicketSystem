# Support Ticket System

An enterprise-grade Support Ticket Backend API built with **.NET 10** using **Clean Architecture** and modern software engineering practices.

## 🚀 Tech Stack & Libraries

- **Framework:** .NET 10.0 (ASP.NET Core Web API)
- **Database:** PostgreSQL (Hosted via Docker Container)
- **ORM:** Entity Framework Core (EF Core) with Fluent API configuration
- **Pattern:** CQRS (Command Query Responsibility Segregation) via **MediatR**
- **Validation:** **FluentValidation** integrated into MediatR pipeline behaviors
- **Object Mapping:** **Mapster** (High-performance mapper)
- **Logging:** **Serilog** for structured logging
- **Testing:** **xUnit**, **FluentAssertions**, and **NSubstitute**

---

## 🏛 Architecture Overview (Clean Architecture)

The project is structured according to Clean/On onion Architecture principles to ensure high testability, maintainability, and decoupling from external frameworks.

- **Domain:** Contains core entities (`Ticket`, `Comment`), enums, and repository interfaces. Independent of any database or external libraries.
- **Application:** Contains business logic, CQRS Commands/Queries, MediatR Handlers, DTOs, and FluentValidation rules.
- **Infrastructure:** Implements data access layer (EF Core DbContext, PostgreSQL mappings, repository implementations) and external services.
- **WebApi:** The entry point. Handles HTTP requests, dependency injection registration, global exception handling, and routing.

---

## 🛠 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- VS Code (with REST Client extension installed for testing)

### Run the Application

1. **Start the PostgreSQL database via Docker:**

   ```bash
   docker compose up -d

   ```

2. **Apply EF Core Migrations: Make sure the dotnet-ef tool is installed, then update the database:**
   dotnet ef database update --project src/SupportTicketSystem.Infrastructure/SupportTicketSystem.Infrastructure.csproj --startup-project src/SupportTicketSystem.WebApi/SupportTicketSystem.WebApi.csproj

3. **Run the API:**
   dotnet run --project src/SupportTicketSystem.WebApi/SupportTicketSystem.WebApi.csproj

🧪 Testing
**Running Unit Tests**
**We use xUnit and NSubstitute to mock repositories and test our CQRS Handlers in isolation:**
dotnet test

📬 API Endpoints & Verification
**You can use the built-in tickets.http file in VS Code (with the REST Client extension) to test the following endpoints:**

**Create a Ticket: POST /api/tickets**
**List All Tickets: GET /api/tickets**
**Get Ticket Details: GET /api/tickets/{id}**

---

### Adım 2: Kodları GitHub'a Gönderme

README dosyasını kaydedip kapattıktan sonra terminale şu komutları sırasıyla yazarak her şeyi GitHub'a yükleyebilirsin:

```bash
# Değişiklikleri ekle
git add .

# Commit mesajı yaz
git commit -m "docs: add comprehensive README.md"

# Kodları gönder
git push -u origin main
```
