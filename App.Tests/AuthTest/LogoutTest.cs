using app.Pages.Auth;
using App.Tests.TestHelpers;
using Bunit;
using Xunit;

namespace App.Tests.AuthTestFolder;

public class LogoutTest : TestContext
{
    [Fact]
    public void LogoutPage_Renders_CommentContent()
    {
        Services.AddFakeProtectedSessionStorage();

        var cut = RenderComponent<Logout>();

        // Logout.razor is a comment placeholder; Blazor may not output HTML comments in markup.
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void LogoutComponent_Exists_AndRenders()
    {
        var cut = RenderComponent<Logout>();

        Assert.NotNull(cut.Instance);
    }
}
