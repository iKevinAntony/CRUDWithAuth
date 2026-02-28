# CRUDWithAuth – Backend API (v1)

## Introduction
CRUDWithAuth is a secure and scalable ASP.NET Core Web API designed to manage expense records with JWT-based authentication and refresh token support. The system follows a clean layered architecture (Controller → Interface → Service) and includes file upload functionality for storing expense proofs such as receipts or documents. It is built using production-ready practices like global exception handling, structured logging, and extensible configurations to ensure maintainability, security, and robustness.

This project demonstrates a real-world backend implementation featuring authentication, authorization, file handling, filtering with pagination, and audit tracking.

---

## Key Features
- JWT Authentication & Authorization (single role model)
- Refresh Token mechanism for secure session renewal
- Global Exception Handling using custom filters
- Clean Architecture: Controller → Interface → Service (Dependency Injection)
- Expense CRUD operations (Create, Read, Update, Delete)
- File Upload support for storing expense proofs (receipts, bills, etc.)
- Filtering & Pagination using stored procedures
- AutoMapper for entity-to-DTO mapping
- Serilog for structured and centralized logging
- Swagger integration for API documentation & testing
- Helpers & Enums for reusable and consistent logic
- Audit tracking (CreatedBy, UpdatedBy, IP Address, Timestamps)

---

## Technologies Used
- ASP.NET Core Web API
- Entity Framework Core
- JWT Bearer Authentication
- AutoMapper
- Serilog Logging
- Dapper (for filtered expense retrieval)
- SQL Server
- Swagger (Swashbuckle)
- IFormFile for file upload handling

---

## Project Architecture
The project follows a clean and maintainable layered architecture:

Controller → Interface → Service → Database

- Controllers: Handle HTTP requests and responses  
- Interfaces: Define service contracts  
- Services: Contain business logic and database interaction  
- Helpers & Enums: Reusable utilities and constants  
- Extensions: Swagger and middleware configurations  

---

## Demo User Credentials
You can use the following demo account to test the APIs:

- **User ID:** user@gmail.com  
- **Password:** User@1234  

> Ensure the database script is executed before using these credentials.

---

## Setup Guide (Run Locally)

### 1. Clone the Repository
```bash
git clone <https://github.com/iKevinAntony/CRUDWithAuth>
cd CRUDWithAuth

### 2. Setup Database
- Navigate to the `DB Script` folder inside the project.
- Open the SQL script file provided in the repository.
- Execute the script in your SQL Server database using SSMS or any SQL client.
- This will create all required tables and stored procedures needed for the application.

### 3. Update Connection String
Open `appsettings.json` and modify the database connection string as per your environment:

```json
"ConnectionStrings": {
  "DBCon": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}