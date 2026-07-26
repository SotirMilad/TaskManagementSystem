using Microsoft.Extensions.Logging.Abstractions;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTOs.Projects.Requests;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.ImplementationServices;
using Xunit;

namespace TaskManagementSystem.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateProject_ShouldCreateProject()
    {
        // arrange
        var context = TestDbContextFactory.Create();

        context.Users.Add(new User
        {
            Id = 1,
            Username = "Test User",
            Email = "test@test.com",
            PasswordHash = "123"
        });

        await context.SaveChangesAsync();

        var service = new ProjectService(
            context,
            NullLogger<ProjectService>.Instance);

        var request = new CreateProjectRequest
        {
            Name = "My First Project",
            Description = "Testing"
        };

        // act
        var result = await service.CreateAsync(1, request);

        // assert
        Assert.NotNull(result);
        Assert.Equal("My First Project", result.Name);
        Assert.Equal(1, context.Projects.Count());
    }

    [Fact]
    public async Task CreateProject_DuplicateName_ShouldThrowConflict()
    {
        // arrange
        var context = TestDbContextFactory.Create();

        context.Users.Add(new User
        {
            Id = 1,
            Username = "Test User",
            Email = "test@test.com",
            PasswordHash = "123"
        });

        context.Projects.Add(new Project
        {
            UserId = 1,
            Name = "Existing Project",
            Description = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var service = new ProjectService(
            context,
            NullLogger<ProjectService>.Instance);

        // act & assert
        await Assert.ThrowsAsync<ApiException>(() =>
            service.CreateAsync(1,
                new CreateProjectRequest
                {
                    Name = "Existing Project",
                    Description = ""
                }));
    }

    [Fact]
    public async Task DeleteProject_ShouldSoftDeleteProject()
    {
        // arrange
        var context = TestDbContextFactory.Create();

        context.Users.Add(new User
        {
            Id = 1,
            Username = "Test User",
            Email = "test@test.com",
            PasswordHash = "123"
        });

        var project = new Project
        {
            UserId = 1,
            Name = "Project",
            Description = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Projects.Add(project);

        await context.SaveChangesAsync();

        var service = new ProjectService(
            context,
            NullLogger<ProjectService>.Instance);

        // act
        await service.DeleteAsync(1, project.Id);

        // assert
        Assert.NotNull(project.DeletedAt);
    }
}