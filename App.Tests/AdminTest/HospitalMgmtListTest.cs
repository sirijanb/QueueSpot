using app.Pages.Admin;
using Bunit;
using app.Models;
using Xunit;

namespace App.Tests.AdminTest;

public class HospitalMgmtListTest : TestContext
{
    private static List<Hospital> CreateTestHospitals()
    {
        return new List<Hospital>
        {
            new Hospital
            {
                Id = 1,
                Name = "General Hospital",
                Address = "100 Health Ave",
                Visible = true,
                Photo = "/images/hospital1.jpg"
            },
            new Hospital
            {
                Id = 2,
                Name = "City Medical Center",
                Address = "200 Care Blvd",
                Visible = false,
                Photo = ""
            }
        };
    }

    [Fact]
    public void HospitalMgmtList_RendersHospitalCards()
    {
        var hospitals = CreateTestHospitals();

        var cut = RenderComponent<HospitalMgmtList>(parameters =>
            parameters.Add(p => p.Hospitals, hospitals));

        Assert.Contains("General Hospital", cut.Markup);
        Assert.Contains("City Medical Center", cut.Markup);
        Assert.Contains("100 Health Ave", cut.Markup);
        Assert.Contains("200 Care Blvd", cut.Markup);
    }

    [Fact]
    public void HospitalMgmtList_ShowsVisibilityStatus()
    {
        var hospitals = CreateTestHospitals();

        var cut = RenderComponent<HospitalMgmtList>(parameters =>
            parameters.Add(p => p.Hospitals, hospitals));

        Assert.Contains("Visible", cut.Markup);
        Assert.Contains("Hidden", cut.Markup);
    }

    [Fact]
    public void HospitalMgmtList_UsesDefaultImage_WhenPhotoEmpty()
    {
        var hospitals = CreateTestHospitals();

        var cut = RenderComponent<HospitalMgmtList>(parameters =>
            parameters.Add(p => p.Hospitals, hospitals));

        // Hospital 2 has empty Photo, should use fallback image
        Assert.Contains("/images/hospital_pic_example_grey.jpg", cut.Markup);
        // Hospital 1 has a custom photo
        Assert.Contains("/images/hospital1.jpg", cut.Markup);
    }

    [Fact]
    public void HospitalMgmtList_RendersEditLinks_WithCorrectHref()
    {
        var hospitals = CreateTestHospitals();

        var cut = RenderComponent<HospitalMgmtList>(parameters =>
            parameters.Add(p => p.Hospitals, hospitals));

        var links = cut.FindAll("a");
        Assert.Contains(links, l => l.GetAttribute("href") == "/admin/hospitals/edit/1");
        Assert.Contains(links, l => l.GetAttribute("href") == "/admin/hospitals/edit/2");
    }

    [Fact]
    public void HospitalMgmtList_RendersEmpty_WhenNoHospitals()
    {
        var cut = RenderComponent<HospitalMgmtList>(parameters =>
            parameters.Add(p => p.Hospitals, new List<Hospital>()));

        var links = cut.FindAll("a");
        Assert.Empty(links);
    }
}
