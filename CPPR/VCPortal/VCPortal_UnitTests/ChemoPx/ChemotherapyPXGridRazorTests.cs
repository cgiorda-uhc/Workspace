
using Bunit;
using System.Diagnostics.Metrics;
using VCPortal_WebUI.Client.Pages.ChemoPx;

namespace VCPortal_UnitTests.ChemoPx;
public class ChemotherapyPXGridRazorTests 
{
    [Fact]
    public void GridShouldPopulateOnLoad()
    {

        //Arrange
        var ctx = new TestContext();
        var cut = ctx.RenderComponent<ChemotherapyPXGrid>();
        //await cut.Instance
        var result = cut.Instance.GridRef;

        //cut.MarkupMatches("<p>Current count: 0</p>");

        //cut.Find("p").MarkupMatches("<p>Current count: 0</p>");

        // Act
        //var element = cut.Find("button");
        //element.Click();

        ////Assert
        //cut.Find("p").MarkupMatches("<p>Current count: 1</p>");


    }
}
