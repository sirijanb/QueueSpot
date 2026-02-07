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

    [Fact]
    public void SignUpPage_ShowsError_WhenEmailEmpty()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        // Fill first name but leave email empty
        cut.FindAll("input[type='text']")[0].Change("John");
        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Contains("email address", alert.TextContent);
    }

    [Fact]
    public void SignUpPage_ShowsError_WhenEmailInvalid()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        cut.FindAll("input[type='text']")[0].Change("John");
        cut.Find("input[type='email']").Change("notanemail");
        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Contains("Invalid email address", alert.TextContent);
    }

    [Fact]
    public void SignUpPage_ShowsError_WhenPasswordMismatch()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        var context = CreateInMemoryContextWithHospitals();
        Services.AddSingleton(context);
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        cut.FindAll("input[type='text']")[0].Change("John");
        cut.Find("input[type='email']").Change("john@mail.com");
        cut.FindAll("input[type='password']")[0].Change("password1");
        cut.FindAll("input[type='password']")[1].Change("password2");
        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Contains("Password does not match", alert.TextContent);
    }

    [Fact]
    public void SignUpPage_ShowsError_WhenDuplicateEmail()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        context.Hospitals.Add(new Hospital { Id = 1, Name = "Test Hospital", Visible = true });
        context.Representative.Add(new Representative
        {
            Id = 1,
            Email = "existing@mail.com",
            Password = "pass",
            Firstname = "Existing",
            Lastname = "User",
            Role = "rep",
            Status = 1
        });
        context.SaveChanges();
        Services.AddSingleton(context);
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        cut.FindAll("input[type='text']")[0].Change("John");
        cut.Find("input[type='email']").Change("existing@mail.com");
        cut.Find("button").Click();

        var alert = cut.Find(".alert-danger");
        Assert.Contains("already registered", alert.TextContent);
    }

    [Fact]
    public void SignUpPage_RendersAllFormFields()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContextWithHospitals());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Sign_Up>();

        Assert.Contains("Contact", cut.Markup);
        Assert.Contains("Hospital", cut.Markup);
        Assert.Contains("Employee ID", cut.Markup);
        Assert.Contains("Password", cut.Markup);
        Assert.Contains("Confirm Password", cut.Markup);
    }
}
