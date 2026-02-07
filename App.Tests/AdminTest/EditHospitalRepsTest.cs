using app.Pages.Admin;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class EditHospitalRepsTest : TestContext
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
            Name = "Test Hospital",
            Visible = true
        };
        context.Hospitals.Add(hospital);

        context.Representative.Add(new Representative
        {
            Id = 1,
            Firstname = "John",
            Lastname = "Doe",
            Email = "john@test.com",
            Contact = "416-555-1234",
            EmployeeID = "EMP001",
            Password = "pass123",
            Role = "rep",
            Status = 1,
            Hospital = hospital
        });

        context.SaveChanges();
        return context;
    }

    private static ApplicationDbContext CreateEmptyContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        context.Hospitals.Add(new Hospital { Id = 1, Name = "Test Hospital", Visible = true });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void EditHospitalRepsPage_Renders_Heading()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>();

        Assert.Contains("Hospital Representative Edit", cut.Markup);
    }

    [Fact]
    public void EditHospitalRepsPage_Renders_FormFields()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>();

        Assert.Contains("First Name", cut.Markup);
        Assert.Contains("Hospital Name", cut.Markup);
        Assert.Contains("Email", cut.Markup);
        Assert.Contains("Contact", cut.Markup);
        Assert.Contains("Hospital", cut.Markup);
        Assert.Contains("Employee ID", cut.Markup);
        Assert.Contains("Approval", cut.Markup);
    }

    [Fact]
    public void EditHospitalRepsPage_LoadsExistingRep()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>(parameters =>
            parameters.Add(p => p.HospitalRepID, 1));

        Assert.Contains("john@test.com", cut.Markup);
    }

    [Fact]
    public void EditHospitalRepsPage_ShowsSaveAndCancelButtons()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>();

        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Save");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Cancel");
    }

    [Fact]
    public void EditHospitalRepsPage_ShowsDeleteButton_WhenEditing()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>(parameters =>
            parameters.Add(p => p.HospitalRepID, 1));

        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Delete");
    }

    [Fact]
    public void EditHospitalRepsPage_HidesDeleteButton_WhenCreatingNew()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital_Reps>();

        var buttons = cut.FindAll("button");
        Assert.DoesNotContain(buttons, b => b.TextContent.Trim() == "Delete");
    }

    [Fact]
    public void EditHospitalRepsPage_CancelNavigatesToRepsList()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();
        var nav = Services.GetRequiredService<NavigationManager>();

        var cut = RenderComponent<Edit_Hospital_Reps>(parameters =>
            parameters.Add(p => p.HospitalRepID, 1));

        var cancelButton = cut.FindAll("button").First(b => b.TextContent.Trim() == "Cancel");
        cancelButton.Click();

        Assert.EndsWith("/admin/reps", nav.Uri);
    }

    [Fact]
    public void EditHospitalRepsPage_SaveNavigatesToRepsList()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        var context = CreateInMemoryContext();
        Services.AddSingleton(context);
        Services.AddFakeProtectedSessionStorage();
        var nav = Services.GetRequiredService<NavigationManager>();

        var cut = RenderComponent<Edit_Hospital_Reps>(parameters =>
            parameters.Add(p => p.HospitalRepID, 1));

        var saveButton = cut.FindAll("button").First(b => b.TextContent.Trim() == "Save");
        saveButton.Click();

        Assert.EndsWith("/admin/reps", nav.Uri);
    }
}
