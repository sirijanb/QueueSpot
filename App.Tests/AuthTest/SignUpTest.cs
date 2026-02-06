using app.Pages.Auth;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AuthTest;

public class SignUpTest : TestContext
{
    private static ApplicationDbContext CreateInMemoryContextWithHospitals()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        context.Hospitals.Add(new Hospital
        {
            Id = 1,
            Name = "Test General Hospital",
            Visible = true
        });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void SignUpPage_Renders_RegistrationForm()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        Assert.Contains("Management Account Registration", cut.Markup);
        Assert.Contains("First Name", cut.Markup);
        Assert.Contains("Last Name", cut.Markup);
        Assert.Contains("Email Address", cut.Markup);
        Assert.Contains("Request registration", cut.Markup);
    }

    [Fact]
    public void SignUpPage_ShowsValidationError_WhenFirstNameEmpty()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.NotNull(alert);
        Assert.Contains("first name", alert.TextContent);
    }

    [Fact]
    public void SignUpPage_ContainsLinkToLogin()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        Assert.Contains("/admin/login", cut.Markup);
        Assert.Contains("Already have an account?", cut.Markup);
    }
}
