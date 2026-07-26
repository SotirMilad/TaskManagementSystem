# Task Management API

A RESTful Task Management API built with **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**. The API allows users to register, authenticate using JWT, manage projects, and manage tasks with filtering, sorting, searching, pagination, and soft deletes.


---

## Local Setup

### 1. Clone the repository

```bash
git clone https://github.com/SotirMilad/TaskManagementSystem.git
cd TaskManagementSystem
```

### 2. Database Configuration

Open **appsettings.json** and update the `DefaultConnection` string to point to your local SQL Server instance.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TaskManagementDB;Integrated Security=True;TrustServerCertificate=True"
}
```

### 3. Run Migrations

Restore the packages and create the database.

```bash
dotnet restore
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

Once it's running, open Swagger UI:

```
https://localhost:7207/swagger
```
---

# Features

- JWT Authentication
- User Registration & Login
- CRUD Operations for Projects
- CRUD Operations for Tasks
- Search Tasks
- Filter Tasks by Status and Priority
- Sort Tasks
- Pagination
- Soft Deletes for Projects and Tasks
- Exception Handling
- Entity Framework Core
- SQL Server Database
- Seed Data
- Unit Testing with xUnit

---

## Seed Data

The application automatically seeds two test users on the first run so you can easily generate a JWT and test the authenticated endpoints.

| Email | Password |
|-------|----------|
| ahmed@test.com | ahmed1234 |
| sara@test.com | sara123456 |

---

# API Overview

## Authentication

### Register

```
POST /api/auth/register
```

Creates a new user.

### Login

```
POST /api/auth/login
```

Authenticates a user and returns a JWT token.

Example request:

```json
{
  "email": "ahmed@test.com",
  "password": "ahmed1234"
}
```

---

## Projects (Requires Bearer Token)

| Method | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/projects` | List all owned projects |
| POST | `/api/projects` | Create a project |
| GET | `/api/projects/{id}` | Get project details |
| PUT | `/api/projects/{id}` | Update a project |
| DELETE | `/api/projects/{id}` | Soft delete a project (updates `DeletedAt`) |

---

## Tasks (Requires Bearer Token)

| Method | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/tasks` | List all tasks |
| GET | `/api/projects/{projectId}/tasks` | Get tasks for a specific project |
| POST | `/api/projects/{projectId}/tasks` | Add a task |
| GET | `/api/tasks/{id}` | Get task details |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Soft delete a task (updates `DeletedAt`) |

### Query Parameters

The `GET /api/tasks` endpoint supports:

- Pagination
- Searching
- Filtering
- Sorting

Example:

```
GET /api/tasks?pageNumber=1&pageSize=5&status=Done&sortBy=DueDate
```

You can also search tasks:

```
GET /api/tasks?q=authentication
```

Or combine multiple query parameters:

```
GET /api/tasks?q=API&status=Done&priority=High&pageNumber=1&pageSize=5
```

---

## Database Schema

The database consists of three main entities:

- **User** → owns multiple Projects.
- **Project** → belongs to one User and contains multiple Tasks.
- **Task** → belongs to one Project.

Projects and Tasks implement **soft delete** using the `DeletedAt` timestamp instead of permanently removing records from the database.

---
# Testing

The project includes unit tests to verify the business logic of the application services.

The tests are written using:

- **xUnit** - Testing framework
- **Entity Framework Core InMemory Database** - Isolated database testing
- **Microsoft.Extensions.Logging.Abstractions** - Mock logging dependency

## Running Tests

To run all tests:

```bash
dotnet test

```
---

# Author

Sotir Milad

Computer Engineering Graduate

ASP.NET Core Developer