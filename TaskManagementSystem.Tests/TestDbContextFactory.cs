using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Context;

namespace TaskManagementSystem.Tests;

public static class TestDbContextFactory
{
    public static ApplicationDBContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDBContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}