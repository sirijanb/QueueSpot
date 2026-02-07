using app.Pages.User;
using Bunit;
using app.Models;
using Xunit;

namespace App.Tests.UserTest;

public class HospitalCardTest : TestContext
{
    private static Hospital CreateTestHospital()
    {
        return new Hospital
        {
            Id = 1,
            Name = "Downtown Medical Center",
            Distance = "2.5 km",
            EstimatedWait = "45",
            BedsOpen = "12",
            Services = new List<string> { "Emergency", "Cardiology" }
        };
    }

    [Fact]
    public void HospitalCard_RendersHospitalName()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Downtown Medical Center", cut.Markup);
    }

    [Fact]
    public void HospitalCard_RendersDistance()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Distance", cut.Markup);
        Assert.Contains("2.5 km", cut.Markup);
    }

    [Fact]
    public void HospitalCard_FormatsWaitTime_UnderOneHour()
    {
        var hospital = CreateTestHospital(); // EstimatedWait = "45"

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Est Wait Time", cut.Markup);
        Assert.Contains("0:45", cut.Markup);
    }

    [Fact]
    public void HospitalCard_FormatsWaitTime_OverOneHour()
    {
        var hospital = CreateTestHospital();
        hospital.EstimatedWait = "90";

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("1:30", cut.Markup);
    }

    [Fact]
    public void HospitalCard_RendersAvailableBeds()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Beds Available", cut.Markup);
        Assert.Contains("12", cut.Markup);
    }

    [Fact]
    public void HospitalCard_RendersServiceBadges()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        var badges = cut.FindAll(".badge");
        Assert.Equal(2, badges.Count);
        Assert.Contains(badges, b => b.TextContent.Trim() == "Emergency");
        Assert.Contains(badges, b => b.TextContent.Trim() == "Cardiology");
    }

    [Fact]
    public void HospitalCard_RendersEmptyServices_WhenNone()
    {
        var hospital = CreateTestHospital();
        hospital.Services = new List<string>();

        var cut = RenderComponent<HospitalCard>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        var badges = cut.FindAll(".badge");
        Assert.Empty(badges);
    }

    [Fact]
    public void HospitalCard_InvokesOnSelect_WhenClicked()
    {
        var hospital = CreateTestHospital();
        Hospital? selectedHospital = null;

        var cut = RenderComponent<HospitalCard>(parameters => parameters
            .Add(p => p.Hospital, hospital)
            .Add(p => p.OnSelect, (Hospital h) => { selectedHospital = h; }));

        cut.Find(".hospital-card").Click();

        Assert.NotNull(selectedHospital);
        Assert.Equal("Downtown Medical Center", selectedHospital!.Name);
    }
}
