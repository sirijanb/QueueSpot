using app.Pages.Admin;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class HospitalsTest : TestContext
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);

        for (int i = 1; i <= 15; i++)
        {
            context.Hospitals.Add(new Hospital
            {
                Id = i,
                Name = $"Hospital {i}",
                Address = $"{i} Main St",
                Visible = true
            });
        }

        context.SaveChanges();
        return context;
    }

    private static ApplicationDbContext CreateEmptyContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void HospitalsPage_Renders_HeadingAndNewButton()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospitals>();

        Assert.Contains("Hospital Management", cut.Markup);
        var newButton = cut.Find("button");
        Assert.Equal("New\n        Hospital", newButton.TextContent.Trim());
    }

    [Fact]
    public void HospitalsPage_LoadsHospitals_OnInit()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospitals>();

        Assert.Contains("Hospital 1", cut.Markup);
        Assert.Contains("Hospital 10", cut.Markup);
    }

    [Fact]
    public void HospitalsPage_ShowsPagination_WhenMultiplePages()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospitals>();

        // 15 hospitals / 10 per page = 2 pages, so pagination should be present
        Assert.Contains("Pagination", cut.Markup);
    }

    [Fact]
    public void HospitalsPage_RendersEmptyList_WhenNoHospitals()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospitals>();

        Assert.Contains("Hospital Management", cut.Markup);
        Assert.DoesNotContain("Hospital 1", cut.Markup);
    }
}
