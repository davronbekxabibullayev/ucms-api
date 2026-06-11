# 🏗️ UCMS — Unified Construction Management System

A REST API for managing construction projects, brigades, work logs, estimates, and payments.

**Stack:** .NET 8 · PostgreSQL · EF Core 8 · ASP.NET Core Identity · JWT

---

## 📁 Project Structure

```
src/
├── Ucms.Domain/          # Entities, enums, interfaces
├── Ucms.Application/     # Services, abstractions
├── Ucms.Infrastructure/  # EF DbContext, migrations, seed data
└── Ucms.Api/             # Controllers, Swagger, Program.cs
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 14+

### 1. Apply migrations

Open **Package Manager Console** in Visual Studio:

```
# Set PMC Default project → Ucms.Infrastructure
# Set VS Startup project  → Ucms.Api

Add-Migration InitialMigration
Update-Database
```

### 3. Seed data

Make sure seed is enabled in `appsettings.json`:

```json
"SeedData": {
  "Enabled": true
}
```

Seed runs automatically on first launch.

---

## 🔐 Authentication

Click **Authorize** in Swagger UI, enter your credentials — JWT is set automatically.

---

## 👥 Test Users

### 🔴 Owner — UCMS (system owner)

Full access across all organizations.

| Name               | Username   | Password       | Role  |
|--------------------|------------|----------------|-------|
| System Super Admin | `sysadmin` | `SysAdmin123!` | Admin |

---

### 🟢 Tenant 1 — Ihtiyor Qurilish Kompaniyasi

Tashkent · `info@demo-qurilish.uz`

| Name                | Username      | Password         | Role       |
|---------------------|---------------|------------------|------------|
| Ahmadov Bahodir     | `admin`       | `Admin123!`      | Admin      |
| Ergashev Jahongir   | `manager`     | `Manager123!`    | Manager    |
| Toshmatov Sherzod   | `brigadir`    | `Brigadir123!`   | Brigadir   |
| Nazarova Gulnora    | `accountant`  | `Accountant123!` | Accountant |

**Projects:** Yunusobod-14 repair *(InProgress)* · Sergeli office *(Planning)*


---

## 🛡️ Roles

| Role        | Permissions                    |
|-------------|--------------------------------|
| Admin       | Full org management            |
| Manager     | Projects, estimates, brigades  |
| Brigadir    | Work log entries               |
| Accountant  | Acts and payments              |

> **Owner** users bypass all organization filters and see data across all tenants.

---

## 📡 API Endpoints

| Method | URL                      | Description         |
|--------|--------------------------|---------------------|
| POST   | `/api/auth/login`        | Login               |
| POST   | `/api/auth/refresh`      | Refresh token       |
| GET    | `/api/organizations`     | List organizations  |
| GET    | `/api/projects`          | List projects       |
| GET    | `/api/brigades`          | List brigades       |
| GET    | `/api/worklogs`          | Work log entries    |
| GET    | `/api/payments/brigade`  | Brigade payments    |
| GET    | `/api/payments/client`   | Client payments     |
| GET    | `/api/clientacts`        | Client acts         |
| GET    | `/api/dashboard`         | Statistics          |
| GET    | `/api/users`             | Users (Admin)       |

Swagger UI: `https://localhost:{PORT}/swagger`

---

## ✍️ Author

**Xabibullayev Davronbek**  
📧 davronbekxabibullayev03.06.88@gmail.com
