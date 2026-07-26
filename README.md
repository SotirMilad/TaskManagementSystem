# Task Management API

A RESTful backend API for managing projects and tasks, built with ASP.NET Core and Entity Framework Core. 

I built this to practice standard API patterns, specifically JWT authentication, soft deletes, and handling complex queries (filtering, sorting, and pagination) backed by a SQL Server database.

## Local Setup

**1. Clone the repo**
```bash
git clone https://github.com/SotirMilad/TaskManagementSystem.git
cd TaskManagementSystem

2. Database Configuration
Open appsettings.json and update the DefaultConnection string to point to your local SQL Server instance.

JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TaskManagementDB;Integrated Security=True;TrustServerCertificate=True"
}
3. Run Migrations
Restore your tools and build the database schema:

Bash
dotnet restore
dotnet ef database update
4. Run the App

Bash
dotnet run
Once it's running, you can hit https://localhost:7207/swagger to test the endpoints on swagger ui.

Seed Data
The app automatically seeds a few test users on the first run so you can easily generate a JWT to test the authenticated routes:

ahmed@test.com / ahmed1234

sara@test.com / sara123456

API Overview
Authentication

POST /api/auth/register - Create a new user

POST /api/auth/login - Authenticate and receive a JWT

Projects (Requires Bearer Token)

GET /api/projects - List all owned projects

POST /api/projects - Create a project

GET /api/projects/{id} - Get project details

PUT /api/projects/{id} - Update a project

DELETE /api/projects/{id} - Soft delete a project (updates DeletedAt)

Tasks (Requires Bearer Token)

GET /api/tasks - List all tasks. Supports pagination, searching, and filtering via query params (e.g., ?pageNumber=1&pageSize=5&status=Done&sortBy=DueDate)

GET /api/projects/{projectId}/tasks - Get tasks for a specific project

POST /api/projects/{projectId}/tasks - Add a task

GET /api/tasks/{id} - Get task details

PUT /api/tasks/{id} - Update a task

DELETE /api/tasks/{id} - Soft delete a task (updates DeletedAt)