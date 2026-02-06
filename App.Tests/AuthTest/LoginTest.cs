using app.Pages.Auth;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AuthTest;

public class LoginTest : TestContext
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        context.Representative.Add(new Representative
        {
            Id = 1,
            Email = "admin@test.com",
            Password = "password123",
            Role = "admin",
            Firstname = "Admin",
            Lastname = "User",
            Status = 1
        });
        context.Representative.Add(new Representative
        {
            Id = 2,
            Email = "rep@test.com",
            Password = "rep456",
            Role = "rep",
            Firstname = "Rep",
            Lastname = "User",
            Status = 1
        });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void LoginPage_Renders_EmailAndPasswordInputs()
    {
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Login>();

        Assert.NotNull(cut.Find("input[type='text']"));
        Assert.NotNull(cut.Find("input[type='password']"));
        Assert.NotNull(cut.Find("button"));
        Assert.Equal("Sign in", cut.Find("button").TextContent.Trim());
    }

    [Fact]
    public void LoginPage_ShowsErrorMessage_WhenInvalidCredentials()
    {
        var dbContext = CreateInMemoryContext();
        Services.AddSingleton(dbContext);
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Login>();

        cut.Find("input[type='text']").Change("wrong@test.com");
        cut.Find("input[type='password']").Change("wrongpass");
        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.NotNull(alert);
        Assert.Contains("Invalid Email or Password", alert.TextContent);
    }

    [Fact]
    public void LoginPage_DisplaysManagementLoginHeading()
    {
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Login>();

        Assert.Contains("Management Login", cut.Markup);
    }
}
