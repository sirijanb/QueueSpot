using app.Pages.Reps;
using App.Tests.TestHelpers;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Xunit;

namespace App.Tests.RepTest;

public class RepHospitalTest : TestContext
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
            Address = "456 Health Blvd",
            BedsOpen = "20",
            Status = "Open",
            EstimatedWait = "30",
            Visible = true,
            Photo = "/images/city_general.jpg"
        };
        context.Hospitals.Add(hospital);

        context.Representative.Add(new Representative
        {
            Id = 1,
            Email = "rep@hospital.com",
            Password = "pass123",
            Firstname = "Rep",
            Lastname = "User",
            Role = "rep",
            Status = 1,
            Hospital = hospital
        });

        context.SaveChanges();
        return context;
    }

    private void SetupServices()
    {
        // Setup JS interop for ProtectedSessionStorage calls in OnAfterRenderAsync
        JSInterop.Setup<string>("sessionStorage.getItem", _ => true).SetResult(null!);
        JSInterop.SetupVoid("sessionStorage.setItem", _ => true);
        Services.AddSingleton(CreateInMemoryContext());
        Services.AddFakeProtectedSessionStorage();
    }

    [Fact]
    public void RepHospitalPage_Renders_Heading()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        Assert.Contains("Hospital Edit", cut.Markup);
    }

    [Fact]
    public void RepHospitalPage_Renders_FormFields()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        Assert.Contains("Beds Available", cut.Markup);
        Assert.Contains("Status", cut.Markup);
        Assert.Contains("Estimated Wait Time (Min)", cut.Markup);
    }

    [Fact]
    public void RepHospitalPage_Renders_UpdateButton()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        var button = cut.Find("button.btn-primary");
        Assert.Equal("Update", button.TextContent.Trim());
    }

    [Fact]
    public void RepHospitalPage_Renders_HospitalImage()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        var img = cut.Find("img");
        Assert.NotNull(img);
        Assert.Equal("Hospital Image", img.GetAttribute("alt"));
    }

    [Fact]
    public void RepHospitalPage_HasThreeInputFields()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        var inputs = cut.FindAll("input[type='text']");
        Assert.Equal(3, inputs.Count);
    }

    [Fact]
    public void RepHospitalPage_DoesNotShowSuccessAlert_Initially()
    {
        SetupServices();

        var cut = RenderComponent<Rep_Hospital>();

        Assert.DoesNotContain("The Hospital Information has been updated", cut.Markup);
    }
}
