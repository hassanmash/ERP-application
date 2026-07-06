# Multi-Tenant Sales ERP

> A scalable, multi-tenant Sales ERP backend built with ASP.NET Core Web API, Entity Framework Core, and PostgreSQL, demonstrating Clean Architecture, tenant isolation, JWT authentication, and extensible Role-Based Access Control.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Objectives](#objectives)
- [Features](#features)
- [Architecture](#architecture)
- [Architecture Flow](#architecture-flow)
- [Project Structure](#project-structure)
- [Core Design Principles](#core-design-principles)
- [Technology Stack](#technology-stack)
- [Implemented Modules](#implemented-modules)
- [Authentication](#authentication)
- [Multi-Tenancy](#multi-tenancy)
- [Authorization & Permissions](#authorization--permissions)
- [Sales Module](#sales-module)
- [Dashboard Module](#dashboard-module)
- [API Overview](#api-overview)
- [Database Design](#database-design)
- [Setup Instructions](#setup-instructions)
- [Environment Variables](#environment-variables)
- [Design Decisions](#design-decisions)
- [Future Improvements](#future-improvements)
- [Screenshots](#screenshots)

---

# Project Overview

This project was developed as part of the **HiLITE Sales OS Technical Assessment**.

The objective was not to build a complete ERP system, but to design and implement a scalable SaaS backend demonstrating sound architectural principles, clean code practices, and multi-tenant application design.

Instead of focusing on implementing every requested feature, this project prioritizes:

- Clean Architecture
- SOLID principles
- Separation of concerns
- Multi-tenancy
- Secure authentication
- Extensible authorization
- Maintainability
- Scalability

The result is a backend that can easily grow into a much larger ERP platform while remaining easy to understand, extend, and maintain.

---

# Objectives

The implementation focuses on demonstrating:

- Multi-tenant SaaS architecture
- Organization data isolation
- JWT Authentication
- PostgreSQL Row-Level Security (RLS)
- EF Core Global Query Filters
- Layered architecture
- Extensible permission system
- Sales lead management
- Dashboard reporting
- Clean API design

---

# Features

## Platform Administration

- Create Organizations
- Activate / Suspend Organizations
- Module Enablement
- Organization Onboarding

---

## Organization Administration

- Team Management
- Role Management
- User Management
- User Activation / Deactivation

---

## Sales

- Lead Management
- Lead Status Tracking
- Activity Management
- Status History
- Assignment Support

---

## Dashboard

- Total Leads
- Lead Status Summary
- Lead Source Summary
- Today's Activities
- Overdue Activities

---

## Security

- JWT Authentication
- Claims-based Identity
- Tenant Resolution Middleware
- Role-based Permission Framework
- PostgreSQL Row-Level Security
- EF Core Global Query Filters

---

# Architecture

The solution follows a layered architecture with clear separation of responsibilities.

```
                HTTP Request
                      │
                      ▼
              ASP.NET Controller
                      │
                      ▼
            Application Service Layer
                      │
                      ▼
              Repository Layer
                      │
                      ▼
              Entity Framework Core
                      │
                      ▼
                 PostgreSQL
```

Each layer has a single responsibility.

| Layer | Responsibility |
|---------|----------------|
| Controllers | HTTP endpoints only |
| Services | Business logic, validation, authorization, DTO mapping |
| Repositories | EF Core queries only |
| DbContext | Database configuration and tenant filtering |
| PostgreSQL | Persistent storage with Row-Level Security |

---

# Architecture Flow

```
HTTP Request
      │
      ▼
Authentication
      │
      ▼
Tenant Resolution Middleware
      │
      ▼
CurrentTenantService
      │
      ▼
Controller
      │
      ▼
Application Service
      │
      ▼
Repository
      │
      ▼
DbContext
      │
      ▼
PostgreSQL
```

Business logic is intentionally centralized inside the Service layer.

Repositories remain responsible only for persistence.

Controllers only coordinate HTTP requests and responses.

---

# Project Structure

```
src
│
├── API
│   ├── Controllers
│   ├── Middleware
│   └── Extensions
│
├── Application
│   ├── DTOs
│   ├── Services
│   ├── Common
│   └── Security
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Repositories
│
├── Infrastructure
│   ├── Persistence
│   ├── Repositories
│   ├── Interceptors
│   └── Migrations
│
└── Shared
```

---

# Core Design Principles

The project intentionally follows several architectural principles.

## Thin Controllers

Controllers contain:

- Routing
- HTTP responses
- Model binding

Controllers **never contain**:

- Business logic
- Validation
- Database access

Every endpoint simply delegates execution to the Service layer.

Example:

```csharp
var result = await _leadService.CreateAsync(request);
return result.ToActionResult(this);
```

---

## Business Logic in Services

Application Services are responsible for:

- Validation
- Transactions
- Authorization
- DTO mapping
- Business rules

This keeps application behavior centralized and easy to test.

---

## Thin Repositories

Repositories only contain Entity Framework Core queries.

Repositories never know:

- JWT
- Current user
- Permissions
- Authorization
- DTOs

This keeps persistence concerns isolated from application logic.

---

## Manual DTO Mapping

DTO mapping is performed manually inside Services.

Example:

```csharp
private LeadResponse MapToResponse(Lead lead)
{
    return new LeadResponse
    {
        Id = lead.Id,
        Name = lead.Name,
        Status = lead.Status
    };
}
```

Manual mapping was chosen over AutoMapper to keep transformations explicit and easy to trace.

---

## ServiceResult Pattern

Every Service returns a `ServiceResult<T>`.

This provides a consistent way to return:

- Success responses
- Validation failures
- Not Found
- Unauthorized
- Business rule violations

Controllers remain extremely small by simply calling:

```csharp
return result.ToActionResult(this);
```

---

# Technology Stack

| Technology | Purpose |
|------------|----------|
| ASP.NET Core Web API | REST API |
| Entity Framework Core | ORM |
| PostgreSQL | Database |
| JWT Authentication | Authentication |
| EF Core Global Query Filters | Tenant isolation |
| PostgreSQL RLS | Database-level tenant security |
| Swagger / OpenAPI | API documentation |
| C# | Backend language |

---

# Implemented Modules

| Module | Status |
|---------|--------|
| Authentication | ✅ |
| Platform Administration | ✅ |
| Organization Management | ✅ |
| Team Management | ✅ |
| Role Management | ✅ |
| User Management | ✅ |
| Sales Lead Management | ✅ |
| Activity Management | ✅ |
| Dashboard Reporting | ✅ |
| Permission Framework | ✅ |
| Multi-Tenancy | ✅ |
| PostgreSQL RLS | ✅ |

---


# Authentication

The application uses **JWT (JSON Web Token)** based authentication to provide stateless and secure access to protected APIs.

After successful login, the server issues a signed JWT containing the authenticated user's identity and tenant information. Every subsequent request includes this token in the `Authorization` header using the Bearer scheme.

```
Authorization: Bearer <JWT_TOKEN>
```

---

## JWT Claims

The token contains the following claims:

| Claim | Description |
|---------|-------------|
| `sub` | User Identifier |
| `email` | User Email |
| `org_id` | Organization Identifier |
| `role_id` | Assigned Role |
| `is_platform_admin` | Platform Administrator Flag |
| `is_org_admin` | Organization Administrator Flag |

These claims provide sufficient context to identify both the authenticated user and the organization they belong to without requiring additional database lookups during request processing.

---

## Authentication Flow

```
                Login Request
                      │
                      ▼
           Validate Credentials
                      │
                      ▼
              Generate JWT Token
                      │
                      ▼
               Return Access Token
                      │
                      ▼
      Client sends JWT with every request
                      │
                      ▼
           JWT Authentication Middleware
                      │
                      ▼
        Tenant Resolution Middleware
                      │
                      ▼
          CurrentTenantService
```

Once authenticated, every request automatically carries the necessary tenant context throughout the application.

---

# Multi-Tenancy

One of the primary objectives of this project is to demonstrate a scalable **multi-tenant SaaS architecture**.

The implementation follows a **Shared Database, Shared Schema** strategy, where all organizations share the same database while each tenant's data is isolated using an `OrganizationId` discriminator.

```
                PostgreSQL Database
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
 Organization A   Organization B   Organization C
```

Every tenant-aware table includes an `OrganizationId` column that uniquely identifies the owning organization.

Examples include:

- Users
- Teams
- Roles
- Leads
- Activities
- Lead Status History

This design allows new organizations to be onboarded without requiring additional databases or schema changes while maintaining complete data isolation.

---

# Tenant Resolution

Each incoming request passes through a custom `TenantResolutionMiddleware`.

The middleware extracts tenant information from the authenticated JWT and populates a scoped `ICurrentTenantService`.

```csharp
public interface ICurrentTenantService
{
    Guid? UserId { get; }

    Guid? OrganizationId { get; }

    bool IsPlatformAdmin { get; }

    bool IsOrgAdmin { get; }
}
```

This service acts as the single source of truth for the authenticated user's tenant context throughout the lifetime of the HTTP request.

Application Services, DbContext, and database interceptors all consume this service instead of directly reading JWT claims.

---

# Two-Layer Tenant Isolation

Tenant isolation is enforced independently at two separate layers.

```
HTTP Request
      │
      ▼
Layer 1
EF Core Global Query Filters
      │
      ▼
Layer 2
PostgreSQL Row Level Security
      │
      ▼
Database
```

This defense-in-depth approach ensures that even if one layer is accidentally bypassed, the second layer continues to protect tenant data.

---

# Layer 1 - EF Core Global Query Filters

Every tenant-aware entity is configured with an EF Core Global Query Filter.

Conceptually, every LINQ query automatically becomes:

```sql
WHERE OrganizationId = CurrentOrganizationId
```

Example configuration:

```csharp
builder.Entity<Lead>()
    .HasQueryFilter(x =>
        _tenant.IsPlatformAdmin ||
        x.OrganizationId == _tenant.OrganizationId);
```

This means developers do not need to manually remember to include tenant filters in every repository query.

Benefits include:

- Automatic tenant filtering
- Cleaner repositories
- Reduced risk of accidental data leakage
- Consistent behavior across the application

---

# Layer 2 - PostgreSQL Row-Level Security (RLS)

To provide an additional layer of protection, PostgreSQL Row-Level Security (RLS) is enabled for every tenant-aware table.

Unlike application-level filtering, RLS is enforced directly by the database engine.

The following SQL policies are applied during database migrations:

```sql
ALTER TABLE Leads ENABLE ROW LEVEL SECURITY;

ALTER TABLE Leads FORCE ROW LEVEL SECURITY;
```

Each table defines policies using:

- `USING`
- `WITH CHECK`

These policies ensure that every `SELECT`, `INSERT`, `UPDATE`, and `DELETE` operation is automatically restricted to the current tenant.

Even if an application query accidentally omits tenant filtering, PostgreSQL continues to enforce organization boundaries.

---

# TenantConnectionInterceptor

Before Entity Framework executes database operations, a custom `TenantConnectionInterceptor` configures the PostgreSQL session.

Conceptually, each connection executes:

```sql
SELECT set_config(
    'app.current_org_id',
    '<organization-id>',
    false
);
```

This makes the current tenant identifier available to PostgreSQL's Row-Level Security policies for the duration of the database session.

Platform Administrators are exempt from tenant filtering, allowing them to perform cross-organization administrative operations.

---

# Why Two Isolation Layers?

Using both EF Core Global Query Filters and PostgreSQL Row-Level Security provides several advantages:

| EF Global Query Filters | PostgreSQL RLS |
|--------------------------|----------------|
| Automatic LINQ filtering | Database-level protection |
| Cleaner repositories | Prevents accidental cross-tenant access |
| Easy developer experience | Independent of application code |
| Improves readability | Defense-in-depth |

This combination ensures that tenant isolation is enforced consistently across both the application and database layers.

---

# Authorization & Permission System

The project includes a flexible permission framework designed to support Role-Based Access Control (RBAC) while remaining extensible for future business requirements.

Rather than relying on hard-coded role names, permissions are stored as JSON within each role.

Example:

```json
{
  "leads_view": "team",
  "dashboard_view": "organization",
  "leads_assign": true,
  "users_manage": false
}
```

This approach allows new permissions to be introduced without requiring database schema changes.

---

# PermissionService

All authorization logic is centralized inside a dedicated `PermissionService`.

Responsibilities include:

- Loading role permissions
- Parsing permission JSON
- Caching permissions during the request
- Evaluating boolean permissions
- Evaluating data scopes
- Applying least-privilege defaults

The service exposes strongly typed methods such as:

```csharp
GetLeadViewScopeAsync()

GetDashboardViewScopeAsync()

CanAssignLeadsAsync()

CanManageUsersAsync()
```

Application Services depend on these methods rather than reading permission data directly.

---

# Permission Parsing

Permissions are stored as a JSON string within the Role entity.

The JSON document is parsed only once per HTTP request using `JsonDocument` and cached inside the scoped `PermissionService`.

This minimizes unnecessary parsing while keeping memory usage efficient.

Invalid or malformed permission data automatically falls back to the most restrictive behavior, following the Principle of Least Privilege.

Fallback rules include:

| Condition | Result |
|------------|--------|
| Missing Boolean Permission | `false` |
| Missing Scope Permission | `Own` |
| Invalid JSON | Minimum Permissions |
| Invalid Value | Minimum Permissions |

---

# Role Independence

Authorization decisions are intentionally **not** based on role names.

For example, the following approach is explicitly avoided:

```csharp
if (role.Name == "Director")
{
    // Access Granted
}
```

Instead, authorization always depends on permissions returned by the `PermissionService`.

This design allows organizations to create custom roles without modifying application code.

---

# Extensibility

The permission framework is intentionally generic.

Adding a new permission requires only:

1. Add a new constant in `PermissionKeys`
2. Add a wrapper method to `PermissionService`
3. Consume the new permission within an Application Service

No database schema changes or repository modifications are required.

This approach keeps the authorization layer maintainable while allowing the system to evolve as new business requirements emerge.

---


# Sales Module

The Sales module forms the core business domain of the application.

It enables organizations to manage sales leads, track customer interactions, and monitor lead progression through a configurable sales pipeline.

All Sales entities inherit from a common `BaseEntity`, providing consistent audit fields across the module.

```text
BaseEntity
├── Id
├── CreatedAt
└── UpdatedAt
```

---

## Lead Management

Each lead belongs to a single organization and represents a potential customer.

### Lead Information

| Field | Description |
|--------|-------------|
| Name | Customer Name |
| Mobile Number | Contact Number |
| Email | Email Address |
| Source | Lead Source |
| Project | Interested Project |
| Assigned User | Sales Executive |
| Status | Current Pipeline Status |

---

## Lead Pipeline

The application supports the following lead lifecycle:

```
New
   │
   ▼
Contacted
   │
   ▼
Visit Scheduled
   │
   ▼
Site Visit Completed
   │
   ▼
Negotiation
   │
   ├──────────────┐
   ▼              ▼
 Won            Lost
```

Every status transition is recorded to maintain a complete history of the lead's lifecycle.

---

## Activity Management

Activities allow sales representatives to record interactions performed against a lead.

Supported activity types include:

- Phone Call
- Meeting
- Site Visit
- Virtual Meeting

Each activity records:

| Field | Description |
|--------|-------------|
| Lead | Associated Lead |
| Activity Type | Type of Interaction |
| Notes | User Notes |
| Activity Date & Time | Scheduled Date |
| Created By | User who created the activity |

---

## Lead Status History

Whenever a lead status changes, an entry is automatically recorded in the `LeadStatusHistory` table.

Each record stores:

- Previous Status
- New Status
- Changed By
- Timestamp

This provides a complete audit trail of every lead's progression through the sales pipeline.

---

# Dashboard Module

The Dashboard module provides organization-level reporting through lightweight aggregation queries.

Instead of returning raw entity data, repositories execute optimized aggregate queries while Application Services map the results into response DTOs.

The Dashboard currently provides:

## Dashboard Summary

Displays:

- Total Leads
- New Leads
- Qualified Leads
- Won Leads
- Lost Leads
- Today's Activities
- Overdue Activities

---

## Lead Status Summary

Returns lead counts grouped by status.

Example:

| Status | Count |
|----------|------:|
| New | 24 |
| Qualified | 18 |
| Won | 9 |
| Lost | 4 |

---

## Lead Source Summary

Returns lead counts grouped by acquisition source.

Example:

| Source | Count |
|---------|------:|
| Website | 34 |
| Referral | 15 |
| Facebook | 12 |
| Walk-in | 6 |

---

# API Overview

The API is organized by business capability rather than technical concern.

## Authentication

| Method | Endpoint |
|---------|----------|
| POST | `/api/auth/login` |
| POST | `/api/auth/refresh-token` |

---

## Platform Administration

| Method | Endpoint |
|---------|----------|
| POST | `/api/platform-admin/organizations` |
| GET | `/api/platform-admin/organizations` |
| GET | `/api/platform-admin/organizations/{id}` |
| PUT | `/api/platform-admin/organizations/{id}` |
| PATCH | `/api/platform-admin/organizations/{id}/status` |
| PATCH | `/api/platform-admin/organizations/{id}/modules` |

---

## Role Management

| Method | Endpoint |
|---------|----------|
| POST | `/api/org-admin/roles` |
| GET | `/api/org-admin/roles` |
| GET | `/api/org-admin/roles/{id}` |
| PUT | `/api/org-admin/roles/{id}` |
| DELETE | `/api/org-admin/roles/{id}` |

---

## Team Management

| Method | Endpoint |
|---------|----------|
| POST | `/api/org-admin/teams` |
| GET | `/api/org-admin/teams` |
| GET | `/api/org-admin/teams/{id}` |
| PUT | `/api/org-admin/teams/{id}` |
| DELETE | `/api/org-admin/teams/{id}` |

---

## User Management

| Method | Endpoint |
|---------|----------|
| POST | `/api/org-admin/users` |
| GET | `/api/org-admin/users` |
| GET | `/api/org-admin/users/{id}` |
| PUT | `/api/org-admin/users/{id}` |
| PATCH | `/api/org-admin/users/{id}/status` |

---

## Lead Management

| Method | Endpoint |
|---------|----------|
| POST | `/api/sales/leads` |
| GET | `/api/sales/leads` |
| GET | `/api/sales/leads/{id}` |
| PUT | `/api/sales/leads/{id}` |
| DELETE | `/api/sales/leads/{id}` |
| PATCH | `/api/sales/leads/{id}/status` |

---

## Activity Management

| Method | Endpoint |
|---------|----------|
| POST | `/api/sales/activities` |
| GET | `/api/sales/activities/{id}` |
| GET | `/api/sales/activities/lead/{leadId}` |
| PUT | `/api/sales/activities/{id}` |
| DELETE | `/api/sales/activities/{id}` |

---

## Dashboard

| Method | Endpoint |
|---------|----------|
| GET | `/api/dashboard/summary` |
| GET | `/api/dashboard/lead-status` |
| GET | `/api/dashboard/lead-sources` |

---

# Database Design

The application uses PostgreSQL with Entity Framework Core.

The data model follows a shared-schema multi-tenant approach where all tenant-aware tables include an `OrganizationId` discriminator.

A simplified relationship diagram is shown below.

```text
Organization
│
├── Users
│     │
│     ├── Role
│     └── Team
│
├── Leads
│      │
│      ├── Activities
│      └── Lead Status History
│
└── Module Configuration
```

Each organization owns its own:

- Users
- Roles
- Teams
- Leads
- Activities
- Dashboard Data

Tenant isolation is enforced using both EF Core Global Query Filters and PostgreSQL Row-Level Security.

---

# Setup Instructions

## Prerequisites

- .NET 10 SDK
- PostgreSQL
- Git

---

## Clone Repository

```bash
git clone https://github.com/hassanmash/ERP-application.git

cd MultiTenantSalesERP
```

---

## Restore Packages

```bash
dotnet restore
```

---

## Configure Database

Update the PostgreSQL connection string inside:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SalesERP;Username=postgres;Password=your_password"
  }
}
```

---

## Apply Migrations

```bash
dotnet ef database update --project ..\Repository\Infrastructure.csproj --startup-project ErpApplicationApi.csproj
```
from project root folder

---

## Run below SQL to seed admin

```sql
-- One-time bootstrap: creates the first platform admin so you can log in
-- and use the (future) Platform Admin endpoints to create real organizations
-- and org-admins through the API instead of SQL from then on.
--
-- Run as the postgres superuser — platform_admin users have
-- organization_id = NULL, and this INSERT bypasses RLS naturally since
-- we're connected as superuser (RLS doesn't apply to inserts you make
-- directly as postgres; this is a one-time exception, not a pattern to
-- repeat for normal data).
 
INSERT INTO users (
    id,
    organization_id,
    team_id,
    role_id,
    name,
    email,
    password_hash,
    is_platform_admin,
    is_org_admin,
    is_active,
    created_at
) VALUES (
    gen_random_uuid(),
    NULL,                                   -- platform admins belong to no org
    NULL,									-- team id
    NULL,									-- role id
    'Platform Administrator',				-- name
    'admin@hiliteos.com',                   -- email
    '$2a$12$NlvfZXXzmWMWPGxMAGsKsePIvu5EO/f6C0BYzj2yAtZFFhHb/Z9Bi', -- BCrypt hash password
    true,                                   -- is_platform_admin
    false,									-- is_org_admin
    true,                                   -- is_active
    now()
);
```
unhashed would be: `HiliteOSAdmin@1101`

---

## Run Application

```bash
dotnet run
```

Swagger will not be available at the moment attaching the collection file to be used in Bruno application( or postman) found at:

```text
./api_collection/
```

---

# Environment Variables

The application supports configuration through `appsettings.json` or environment variables.

| Key | Description |
|------|-------------|
| ConnectionStrings__DefaultConnection | PostgreSQL Connection String |
| Jwt__Issuer | JWT Issuer |
| Jwt__Audience | JWT Audience |
| Jwt__SecretKey | JWT Signing Key |
| Jwt__ExpiryMinutes | Token Expiry |
| ASPNETCORE_ENVIRONMENT | Application Environment |

---

# Error Handling

The application follows a consistent error handling strategy using the `ServiceResult<T>` pattern.

Rather than throwing exceptions for expected business scenarios, services return standardized success or failure responses, which controllers translate into appropriate HTTP status codes using `ToActionResult()`.

This keeps controllers lightweight while ensuring consistent API responses across the application.

---


# Design Decisions

This project intentionally prioritizes architecture, maintainability, and extensibility over implementing every possible feature. The following design decisions were made to keep the codebase clean, scalable, and easy to evolve.

---

## Clean Layered Architecture

The solution follows a traditional layered architecture:

```
Controller
    │
    ▼
Service
    │
    ▼
Repository
    │
    ▼
Entity Framework Core
    │
    ▼
PostgreSQL
```

Each layer has a clearly defined responsibility.

| Layer | Responsibility |
|--------|----------------|
| Controller | HTTP endpoints and request/response handling |
| Service | Business logic, validation, authorization, transactions, DTO mapping |
| Repository | Data access using Entity Framework Core |
| DbContext | Entity configuration, migrations, tenant filtering |

This separation keeps the codebase maintainable while reducing coupling between business logic and persistence.

---

## Manual DTO Mapping

DTO mapping is performed manually inside the Service layer.

Although libraries such as AutoMapper reduce boilerplate, manual mapping was chosen because it:

- Makes transformations explicit
- Simplifies debugging
- Avoids hidden conventions
- Improves readability for technical reviewers
- Keeps dependencies minimal

---

## Thin Repositories

Repositories are intentionally limited to persistence operations.

Repositories do **not** contain:

- Business rules
- Authorization
- Validation
- DTO mapping
- Current user logic

This keeps data access isolated and reusable.

---

## Business Logic in Services

Application Services are responsible for:

- Validation
- Business rules
- Authorization
- Transactions
- DTO mapping
- Coordination between repositories

This centralizes application behavior and keeps controllers and repositories lightweight.

---

## ServiceResult Pattern

All services return a standardized `ServiceResult<T>` (or `ServiceResult`) rather than directly returning entities or throwing exceptions for expected business scenarios.

Benefits include:

- Consistent API responses
- Centralized success and failure handling
- Minimal controller logic
- Simplified error handling

Controllers simply delegate the response conversion:

```csharp
return result.ToActionResult(this);
```

---

## Multi-Tenant Security

Tenant isolation is implemented using two independent layers:

1. **EF Core Global Query Filters**
2. **PostgreSQL Row-Level Security (RLS)**

This defense-in-depth strategy protects tenant data at both the application and database levels.

---

## Permission-Based Authorization

Authorization is permission-driven rather than role-name driven.

Instead of checking role names such as:

```csharp
if (role.Name == "Director")
```

authorization is always performed through the `PermissionService`.

This allows organizations to define custom roles without requiring code changes.

---

## Shared Database Strategy

The project uses a **Shared Database / Shared Schema** multi-tenant strategy.

Advantages include:

- Simple deployment
- Efficient resource utilization
- Easy onboarding of new organizations
- Minimal operational overhead
- Scalable for small and medium SaaS platforms

---

## Extensibility

The architecture is designed to support future expansion with minimal changes.

Examples include:

- Additional ERP modules
- New permissions
- Reporting
- Notifications
- Workflow automation
- Background processing

Most new features can be added by introducing new Services and Repositories without impacting existing modules.

---

# Scalability Considerations

Although the project is an MVP, several architectural decisions support future scalability.

## Horizontal Scaling

Because authentication is JWT-based and stateless, multiple API instances can be deployed behind a load balancer without requiring sticky sessions.

---

## Database Security

PostgreSQL Row-Level Security ensures tenant isolation even if application-level filtering is accidentally omitted.

---

## Modular Design

Each business capability is isolated into its own module.

Examples:

- Authentication
- Platform Administration
- Organization Administration
- Sales
- Dashboard

This structure simplifies maintenance and future feature development.

---

## Extensible Permission Framework

The permission system is intentionally generic.

Adding a new permission typically requires:

1. Add a permission key
2. Add a wrapper method in `PermissionService`
3. Consume the permission inside an Application Service

No schema changes are required.

---

# Future Enhancements

The current implementation focuses on demonstrating architectural concepts and the core business modules.

Several enhancements can be added without major architectural changes.

## Role-Based Access Control

- Enforce permissions across every endpoint
- Dynamic permission management
- Custom permission groups

---

## Notifications

Implement an event-driven notification system using technologies such as:

- Domain Events
- RabbitMQ
- Azure Service Bus
- Kafka

Examples:

- Lead Created
- Lead Assigned
- Lead Won
- Activity Reminder

This keeps the Sales module loosely coupled from notification delivery.

---

## Sales Enhancements

Potential additions include:

- Customer Management
- Opportunity Tracking
- Quotations
- Follow-up Scheduling
- Lead Assignment Strategies
- Sales Targets
- Conversion Analytics

---

## Dashboard Enhancements

- Role-aware dashboards
- Team leaderboards
- Executive performance
- Conversion trends
- Monthly analytics
- Charts and visualizations

---

## Audit Logging

Introduce a centralized audit trail to record:

- User actions
- Entity changes
- Authentication events
- Administrative operations

---

## Background Jobs

Examples:

- Reminder notifications
- Scheduled reports
- Data cleanup
- Email processing

---

## Frontend

A modern Angular application can be added using the existing REST API without requiring backend architectural changes.

---

## Testing

Future improvements include:

- Unit Tests
- Integration Tests
- Repository Tests
- End-to-End API Tests

---

## Docker Support

Containerization can simplify deployment by introducing:

- Docker
- Docker Compose
- CI/CD pipelines

---

# Assessment Requirement Mapping

The following table summarizes the implementation status against the provided technical assessment.

| Requirement | Status |
|-------------|--------|
| Multi-Tenant Architecture | ✅ Implemented |
| JWT Authentication | ✅ Implemented |
| Platform Administration | ✅ Implemented |
| Organization Administration | ✅ Implemented |
| Team Management | ✅ Implemented |
| Role Management | ✅ Implemented |
| User Management | ✅ Implemented |
| Lead Management | ✅ Implemented |
| Activity Management | ✅ Implemented |
| Lead Status Tracking | ✅ Implemented |
| Dashboard & Analytics | ✅ Implemented |
| PostgreSQL Row-Level Security | ✅ Implemented |
| EF Core Global Query Filters | ✅ Implemented |
| Permission Framework | ✅ Foundation Implemented |
| Role-Based Data Scope | ⚠️ Framework Implemented (ready for integration across all endpoints) |
| Event-Based Notifications | ⏳ Planned |
| Angular Frontend | ⏳ Planned |
| Real-Time Notifications | ⏳ Planned |
| Automated Tests | ⏳ Planned |
| Docker Deployment | ⏳ Planned |

---

# Key Takeaways

This project demonstrates the implementation of a scalable multi-tenant SaaS backend with a strong focus on software architecture rather than feature quantity.

Key architectural highlights include:

- Clean layered architecture
- SOLID design principles
- JWT-based authentication
- Shared database multi-tenancy
- EF Core Global Query Filters
- PostgreSQL Row-Level Security
- Extensible permission framework
- Thin repositories
- Service-oriented business logic
- Consistent API response pattern
- Dashboard reporting
- Modular and maintainable codebase

The project provides a solid foundation that can be extended into a complete ERP platform with additional business modules while preserving the existing architectural boundaries.

---

# Screenshots

Login: <img width="1364" height="763" alt="image" src="https://github.com/user-attachments/assets/92727ed1-0928-481e-aff1-4a2713828ead" />


All users under logged user: <img width="1368" height="739" alt="image" src="https://github.com/user-attachments/assets/b1e6648a-9c9a-4115-b0cb-f1ab11d0f948" />

All activities done by a lead: <img width="1368" height="755" alt="image" src="https://github.com/user-attachments/assets/d939cf17-4d8a-4ac9-b4f6-9eeb764efdeb" />


---

# Conclusion

This implementation was developed to demonstrate architectural thinking, clean code practices, and scalable backend design as part of the HiLITE Sales OS Technical Assessment.

Rather than maximizing feature count, the focus was placed on building a maintainable, extensible, and secure foundation that showcases modern backend engineering practices.

The resulting architecture supports future growth while maintaining clear separation of concerns, strong tenant isolation, and a clean development experience.

---

**Thank you for reviewing this project. Feedback and suggestions are always welcome.**
