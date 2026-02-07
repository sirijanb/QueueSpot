using app.Pages.Admin;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class EditHospitalTest : TestContext
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
            Address = "123 Main St",
            BedsOpen = "10",
            Status = "Open",
            Visible = true,
            Photo = "/images/test.jpg"
        };
        context.Hospitals.Add(hospital);

        var service = new HospitalService { Id = 1, ServiceName = "Emergency" };
        context.HospitalServices.Add(service);

        context.HospitalServiceAssignment.Add(new HospitalServiceAssignment
        {
            Id = 1,
            Hospital = hospital,
            Service = service
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
        context.HospitalServices.Add(new HospitalService { Id = 1, ServiceName = "Emergency" });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void EditHospitalPage_Renders_FormFields()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        Assert.Contains("Hospital Edit", cut.Markup);
        Assert.Contains("Hospital Name", cut.Markup);
        Assert.Contains("Address", cut.Markup);
        Assert.Contains("Beds Available", cut.Markup);
        Assert.Contains("Status", cut.Markup);
        Assert.Contains("Services", cut.Markup);
        Assert.Contains("Visible", cut.Markup);
    }

    [Fact]
    public void EditHospitalPage_LoadsExistingHospital_WhenIdProvided()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        Assert.Contains("Test Hospital", cut.Markup);
        Assert.Contains("123 Main St", cut.Markup);
    }

    [Fact]
    public void EditHospitalPage_ShowsSaveAndCancelButtons()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Save");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Cancel");
    }

    [Fact]
    public void EditHospitalPage_ShowsDeleteButton_WhenEditingExisting()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Trim() == "Delete");
    }

    [Fact]
    public void EditHospitalPage_HidesDeleteButton_WhenCreatingNew()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>();

        var buttons = cut.FindAll("button");
        Assert.DoesNotContain(buttons, b => b.TextContent.Trim() == "Delete");
    }

    [Fact]
    public void EditHospitalPage_DisplaysAssignedServices()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.button.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        Assert.Contains("Emergency", cut.Markup);
    }

    [Fact]
    public void EditHospitalPage_SaveCreatesNewHospital()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        var context = CreateEmptyContext();
        Services.AddSingleton(context);
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>();

        cut.FindAll("input[type='text']")[0].Change("New Hospital");
        cut.FindAll("input[type='text']")[1].Change("999 New Ave");

        cut.InvokeAsync(() => cut.FindAll("button").First(b => b.TextContent.Trim() == "Save").Click());

        Assert.Equal(1, context.Hospitals.Count());
        Assert.Equal("New Hospital", context.Hospitals.First().Name);
    }

    [Fact]
    public void EditHospitalPage_CancelNavigatesToHospitalsList()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();
        var nav = Services.GetRequiredService<NavigationManager>();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        var cancelButton = cut.FindAll("button").First(b => b.TextContent.Trim() == "Cancel");
        cancelButton.Click();

        Assert.EndsWith("/admin/hospitals", nav.Uri);
    }

    [Fact]
    public void EditHospitalPage_SaveUpdatesExistingHospital()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        var context = CreateInMemoryContext();
        Services.AddSingleton(context);
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        cut.FindAll("input[type='text']")[0].Change("Updated Hospital");

        cut.InvokeAsync(() => cut.FindAll("button").First(b => b.TextContent.Trim() == "Save").Click());

        var hospital = context.Hospitals.First();
        Assert.Equal("Updated Hospital", hospital.Name);
    }

    [Fact]
    public void EditHospitalPage_UsesCustomPhoto_WhenAvailable()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>(parameters =>
            parameters.Add(p => p.HospitalID, 1));

        var img = cut.Find("img");
        Assert.Equal("/images/test.jpg", img.GetAttribute("src"));
    }

    [Fact]
    public void EditHospitalPage_UsesDefaultPhoto_WhenNoPhoto()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.autocomplete.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.switch.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Edit_Hospital>();

        var img = cut.Find("img");
        Assert.Equal("/images/hospital_pic_example_grey.jpg", img.GetAttribute("src"));
    }
}
