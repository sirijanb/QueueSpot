using app.Pages.Admin;
using Bunit;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class RepMgmtListTest : TestContext
{
    private static List<Representative> CreateTestRepresentatives()
    {
        var hospital = new Hospital
        {
            Id = 1,
            Name = "General Hospital"
        };

        return new List<Representative>
        {
            new Representative
            {
                Id = 1,
                Firstname = "John",
                Lastname = "Doe",
                Email = "john@test.com",
                Role = "rep",
                Status = 1,
                Hospital = hospital
            },
            new Representative
            {
                Id = 2,
                Firstname = "Jane",
                Lastname = "Smith",
                Email = "jane@test.com",
                Role = "rep",
                Status = 1,
                Hospital = null!
            }
        };
    }

    [Fact]
    public void RepMgmtList_RendersRepresentativeNames()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);

        var cut = RenderComponent<RepMgmtList>(parameters =>
            parameters.Add(p => p.Representatives, CreateTestRepresentatives()));

        Assert.Contains("John", cut.Markup);
        Assert.Contains("Doe", cut.Markup);
        Assert.Contains("Jane", cut.Markup);
        Assert.Contains("Smith", cut.Markup);
    }

    [Fact]
    public void RepMgmtList_ShowsApprovedBadge()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);

        var cut = RenderComponent<RepMgmtList>(parameters =>
            parameters.Add(p => p.Representatives, CreateTestRepresentatives()));

        Assert.Contains("Approved", cut.Markup);
    }

    [Fact]
    public void RepMgmtList_ShowsHospitalName_WhenAvailable()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);

        var cut = RenderComponent<RepMgmtList>(parameters =>
            parameters.Add(p => p.Representatives, CreateTestRepresentatives()));

        Assert.Contains("General Hospital", cut.Markup);
    }

    [Fact]
    public void RepMgmtList_HandlesNullHospital()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);

        var reps = new List<Representative>
        {
            new Representative
            {
                Id = 1,
                Firstname = "Solo",
                Lastname = "Rep",
                Email = "solo@test.com",
                Role = "rep",
                Status = 1,
                Hospital = null!
            }
        };

        var cut = RenderComponent<RepMgmtList>(parameters =>
            parameters.Add(p => p.Representatives, reps));

        Assert.Contains("Solo", cut.Markup);
        Assert.Contains("Rep", cut.Markup);
    }

    [Fact]
    public void RepMgmtList_RendersEmpty_WhenNoRepresentatives()
    {
        JSInterop.SetupVoid("window.blazorBootstrap.card.initialize", _ => true);

        var cut = RenderComponent<RepMgmtList>(parameters =>
            parameters.Add(p => p.Representatives, new List<Representative>()));

        var listItems = cut.FindAll("li");
        Assert.Empty(listItems);
    }
}
