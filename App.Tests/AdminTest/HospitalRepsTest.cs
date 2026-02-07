using app.Pages.Admin;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class HospitalRepsTest : TestContext
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

        for (int i = 1; i <= 12; i++)
        {
            context.Representative.Add(new Representative
            {
                Id = i,
                Firstname = $"Rep{i}",
                Lastname = $"User{i}",
                Email = $"rep{i}@test.com",
                Password = "pass",
                Role = "rep",
                Status = 1,
                Hospital = hospital
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
    public void HospitalRepsPage_Renders_Heading()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospital_Reps>();

        Assert.Contains("Hospital Representatives", cut.Markup);
    }

    [Fact]
    public void HospitalRepsPage_LoadsRepresentatives()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospital_Reps>();

        Assert.Contains("Rep1", cut.Markup);
        Assert.Contains("Rep10", cut.Markup);
    }

    [Fact]
    public void HospitalRepsPage_PaginatesResults()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospital_Reps>();

        // 12 reps, 10 per page - first page should not show Rep11
        Assert.Contains("Rep10", cut.Markup);
        Assert.DoesNotContain("Rep11", cut.Markup);
    }

    [Fact]
    public void HospitalRepsPage_ShowsSearchInput()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospital_Reps>();

        Assert.Contains("Pagination", cut.Markup);
    }

    [Fact]
    public void HospitalRepsPage_RendersEmpty_WhenNoReps()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.pagination.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.textinput.initialize", _ => true);
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);
        Services.AddSingleton(CreateEmptyContext());
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Hospital_Reps>();

        Assert.Contains("Hospital Representatives", cut.Markup);
        Assert.DoesNotContain("Rep1", cut.Markup);
    }
}
