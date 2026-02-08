using app.Pages.User;
using Bunit;
using app.Models;
using Xunit;

namespace App.Tests.UserTest;

public class HospitalDetailsTest : TestContext
{
    private static Hospital CreateTestHospital()
    {
        return new Hospital
        {
            Id = 1,
            Name = "Downtown Medical Center",
            Status = "Open",
            EstimatedWait = "45",
            BedsOpen = "12",
            Services = new List<string> { "Emergency", "Cardiology", "Radiology" }
        };
    }

    private static Hospital CreateHospitalWithLongWait()
    {
        return new Hospital
        {
            Id = 2,
            Name = "Uptown Hospital",
            Status = "Busy",
            EstimatedWait = "90",
            BedsOpen = "3",
            Services = new List<string> { "Emergency" }
        };
    }

    [Fact]
    public void HospitalDetails_RendersHospitalName()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Downtown Medical Center", cut.Markup);
    }

    [Fact]
    public void HospitalDetails_RendersStatusBadge()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        var badge = cut.Find(".badge.bg-success");
        Assert.Equal("Open", badge.TextContent.Trim());
    }

    [Fact]
    public void HospitalDetails_RendersCloseButton()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        var closeButton = cut.Find(".close-button");
        Assert.NotNull(closeButton);
        Assert.Equal("×", closeButton.TextContent);
    }

    [Fact]
    public void HospitalDetails_FormatsWaitTime_UnderOneHour()
    {
        var hospital = CreateTestHospital(); // EstimatedWait = "45"

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Current ER Wait Time", cut.Markup);
        Assert.Contains("0:45", cut.Markup);
    }

    [Fact]
    public void HospitalDetails_FormatsWaitTime_OverOneHour()
    {
        var hospital = CreateHospitalWithLongWait(); // EstimatedWait = "90"

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("1:30", cut.Markup);
    }

    [Fact]
    public void HospitalDetails_RendersAvailableBeds()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Available beds", cut.Markup);
        Assert.Contains("12", cut.Markup);
    }

    [Fact]
    public void HospitalDetails_RendersOccupancyStatus()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Occupancy Status", cut.Markup);
        Assert.Contains("00%", cut.Markup);
    }

    [Fact]
    public void HospitalDetails_RendersServices()
    {
        var hospital = CreateTestHospital();

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        Assert.Contains("Services", cut.Markup);
        var badges = cut.FindAll(".badge.bg-primary");
        Assert.Equal(3, badges.Count);
        Assert.Contains(badges, b => b.TextContent.Trim() == "Emergency");
        Assert.Contains(badges, b => b.TextContent.Trim() == "Cardiology");
        Assert.Contains(badges, b => b.TextContent.Trim() == "Radiology");
    }

    [Fact]
    public void HospitalDetails_RendersEmptyServices_WhenNone()
    {
        var hospital = new Hospital
        {
            Id = 3,
            Name = "Small Clinic",
            Status = "Open",
            EstimatedWait = "10",
            BedsOpen = "2",
            Services = new List<string>()
        };

        var cut = RenderComponent<HospitalDetails>(parameters =>
            parameters.Add(p => p.Hospital, hospital));

        var badges = cut.FindAll(".badge.bg-primary");
        Assert.Empty(badges);
    }

    [Fact]
    public void HospitalDetails_InvokesOnClickClose_WhenCloseClicked()
    {
        var hospital = CreateTestHospital();
        bool closeCalled = false;

        var cut = RenderComponent<HospitalDetails>(parameters => parameters
            .Add(p => p.Hospital, hospital)
            .Add(p => p.OnClickClose, () => { closeCalled = true; }));

        cut.Find(".close-button").Click();

        Assert.True(closeCalled);
    }
}
