using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Tasks.Requests;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.ImplementationServices;
using Xunit;

namespace TaskManagementSystem.Tests;

public class TaskServiceTests
{
    private async Task<(ApplicationDBContext context, Project project)> SetupDatabase()
    {
        var context = TestDbContextFactory.Create();

        var user = new User
        {
            Id = 1,
            Username = "TestUser",
            Email = "test@test.com",
            PasswordHash = "123"
        };

        var project = new Project
        {
            UserId = 1,
            Name = "Project 1",
            Description = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Projects.Add(project);

        await context.SaveChangesAsync();

        return (context, project);
    }

    [Fact]
    public async Task CreateTask_ShouldCreateTask()
    {
        // arrange
        var (context, project) = await SetupDatabase();

        var service = new TaskService(
            context,
            NullLogger<TaskService>.Instance);

        var request = new CreateTaskRequest
        {
            Title = "Finish API",
            Description = "Testing",
            Status = "Todo",
            Priority = "High"
        };

        // act
        var result = await service.CreateAsync(1, project.Id, request);

        // assert
        Assert.NotNull(result);
        Assert.Equal("Finish API", result.Title);
        Assert.Single(context.Tasks);
    }

    [Fact]
    public async Task GetAll_FilterByStatus_ShouldReturnOnlyMatchingTasks()
    {
        // arrange
        var (context, project) = await SetupDatabase();

        context.Tasks.AddRange(
            new TaskItem
            {
                ProjectId = project.Id,
                Title = "Task 1",
                Status = TaskState.Todo,
                Priority = TaskPriority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project.Id,
                Title = "Task 2",
                Status = TaskState.Done,
                Priority = TaskPriority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var service = new TaskService(
            context,
            NullLogger<TaskService>.Instance);

        // act
        var result = await service.GetAllAsync(1, new TaskQueryParameters
        {
            Status = "Done"
        });

        // assert
        Assert.Single(result.Items);
        Assert.Equal("Task 2", result.Items.First().Title);
    }

    [Fact]
    public async Task GetAll_SearchByTitle_ShouldReturnMatchingTask()
    {
        // arrange
        var (context, project) = await SetupDatabase();

        context.Tasks.AddRange(
            new TaskItem
            {
                ProjectId = project.Id,
                Title = "Buy supplies",
                Status = TaskState.Todo,
                Priority = TaskPriority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                ProjectId = project.Id,
                Title = "Finish Backend",
                Status = TaskState.Todo,
                Priority = TaskPriority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var service = new TaskService(
            context,
            NullLogger<TaskService>.Instance);

        // act
        var result = await service.GetAllAsync(1, new TaskQueryParameters
        {
            Search = "backend"
        });

        // assert
        Assert.Single(result.Items);
        Assert.Equal("Finish Backend", result.Items.First().Title);
    }
}