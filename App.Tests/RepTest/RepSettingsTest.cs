using app.Pages.Reps;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.RepTest;

public class RepSettingsTest : TestContext
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);

        var hospital = new Hospital
        {
            Id = 1,
            Name = "City General",
            Visible = true
        };
        context.Hospitals.Add(hospital);

        context.Representative.Add(new Representative
        {
            Id = 1,
            Firstname = "Jane",
            Lastname = "Doe",
            Email = "jane@test.com",
            Contact = "416-555-9999",
            EmployeeID = "EMP100",
            Password = "oldpass",
            Role = "rep",
            Status = 1,
            Hospital = hospital
        });

        context.SaveChanges();
        return context;
    }

    private void SetupServices()
    {
        JSInterop.Setup<string>("sessionStorage.getItem", _ => true).SetResult(null!);
        JSInterop.SetupVoid("sessionStorage.setItem", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();
    }

    [Fact]
    public void RepSettingsPage_Renders_Heading()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        Assert.Contains("Hospital Representative Edit", cut.Markup);
    }

    [Fact]
    public void RepSettingsPage_Renders_AllFormFields()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        Assert.Contains("First Name", cut.Markup);
        Assert.Contains("Last Name", cut.Markup);
        Assert.Contains("Email", cut.Markup);
        Assert.Contains("Contact", cut.Markup);
        Assert.Contains("Hospital", cut.Markup);
        Assert.Contains("Employee ID", cut.Markup);
        Assert.Contains("Password", cut.Markup);
        Assert.Contains("Password(Confirmation)", cut.Markup);
    }

    [Fact]
    public void RepSettingsPage_Renders_SaveButton()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        var button = cut.Find("button.btn-primary");
        Assert.Equal("Save", button.TextContent.Trim());
    }

    [Fact]
    public void RepSettingsPage_HasPasswordFields()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        var passwordFields = cut.FindAll("input[type='password']");
        Assert.Equal(2, passwordFields.Count);
    }

    [Fact]
    public void RepSettingsPage_HasReadonlyEmailField()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        var emailInput = cut.Find("input[type='email']");
        Assert.NotNull(emailInput.GetAttribute("readonly"));
    }

    [Fact]
    public void RepSettingsPage_DoesNotShowAlerts_Initially()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Settings>();

        Assert.DoesNotContain("alert-danger", cut.Markup);
        Assert.DoesNotContain("alert-success", cut.Markup);
    }
}
