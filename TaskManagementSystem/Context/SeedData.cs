using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Context
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDBContext context)
        {

            // seeding willnot be applied if our databasae have data in it
            if (await context.Users.AnyAsync())
                return;

            var hasher = new PasswordHasher<User>();

            var user1 = new User
            {
                Username = "Ahmed",
                Email = "ahmed@test.com"
            };
            user1.PasswordHash = hasher.HashPassword(user1, "ahmed1234");

            var user2 = new User
            {
                Username = "Sara",
                Email = "sara@test.com"
            };
            user2.PasswordHash = hasher.HashPassword(user2, "sara123456");

            context.Users.AddRange(user1, user2);

            await context.SaveChangesAsync();

            var project1 = new Project
            {
                UserId = user1.Id,
                Name = "Task Management API",
                Description = "Interview Project",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var project2 = new Project
            {
                UserId = user1.Id,
                Name = "E-Commerce API",
                Description = "Shopping Backend",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var project3 = new Project
            {
                UserId = user1.Id,
                Name = "Hospital System",
                Description = "Clinic Management",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var project4 = new Project
            {
                UserId = user2.Id,
                Name = "Personal Website",
                Description = "Portfolio",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var project5 = new Project
            {
                UserId = user2.Id,
                Name = "Mobile App",
                Description = "Fitness Tracker",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Projects.AddRange(
                project1,
                project2,
                project3,
                project4,
                project5);

            await context.SaveChangesAsync();

            context.Tasks.AddRange(

            // Project 1
            new TaskItem
            {
                ProjectId = project1.Id,
                Title = "Design Database",
                Description = "Create EF Core models",
                Status = TaskState.Done,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project1.Id,
                Title = "Implement Authentication",
                Description = "JWT Authentication",
                Status = TaskState.InProgress,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project1.Id,
                Title = "Write Documentation",
                Description = "Swagger Documentation",
                Status = TaskState.Todo,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Project 2
            new TaskItem
            {
                ProjectId = project2.Id,
                Title = "Create Products API",
                Description = "CRUD Endpoints",
                Status = TaskState.Done,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project2.Id,
                Title = "Shopping Cart",
                Description = "Cart Logic",
                Status = TaskState.InProgress,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project2.Id,
                Title = "Payment Integration",
                Description = "Stripe API",
                Status = TaskState.Todo,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(9)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Project 3
            new TaskItem
            {
                ProjectId = project3.Id,
                Title = "Patient Module",
                Description = "Patient CRUD",
                Status = TaskState.Done,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project3.Id,
                Title = "Appointments",
                Description = "Appointment Scheduling",
                Status = TaskState.InProgress,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(8)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project3.Id,
                Title = "Medical Reports",
                Description = "PDF Reports",
                Status = TaskState.Todo,
                Priority = TaskPriority.Low,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(12)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Project 4
            new TaskItem
            {
                ProjectId = project4.Id,
                Title = "Landing Page",
                Description = "Responsive Design",
                Status = TaskState.Done,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project4.Id,
                Title = "About Page",
                Description = "Personal Information",
                Status = TaskState.InProgress,
                Priority = TaskPriority.Low,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project4.Id,
                Title = "Contact Form",
                Description = "Email Integration",
                Status = TaskState.Todo,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Project 5
            new TaskItem
            {
                ProjectId = project5.Id,
                Title = "Login Screen",
                Description = "User Authentication",
                Status = TaskState.Done,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project5.Id,
                Title = "Workout Tracking",
                Description = "Track Exercises",
                Status = TaskState.InProgress,
                Priority = TaskPriority.High,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project5.Id,
                Title = "Push Notifications",
                Description = "Reminder Notifications",
                Status = TaskState.Todo,
                Priority = TaskPriority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

            await context.SaveChangesAsync();
        }
    }
}