using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Semver;

namespace CSF.Extensions.WebDriver.Identification;

[TestFixture,Parallelizable]
public class BrowserIdFactoryTests
{
    [Test,AutoMoqData]
    public void GetBrowserIdShouldReturnACorrectIdWhenTheWebDriverHasTheAppropriateCapabilities(ICapabilities caps, BrowserIdFactory sut)
    {
        var driver = new Mock<IWebDriver>();
        driver.As<IHasCapabilities>().SetupGet(x => x.Capabilities).Returns(caps);
        Mock.Get(caps).Setup(x => x.GetCapability("browserName")).Returns("FooBrowser");
        Mock.Get(caps).Setup(x => x.GetCapability("platformName")).Returns("BarPlatform");
        Mock.Get(caps).Setup(x => x.GetCapability("browserVersion")).Returns("1.2.3");

        var expected = new BrowserId("FooBrowser",
                                     "BarPlatform",
                                     new SemanticBrowserVersion(SemVersion.Parse("1.2.3", SemVersionStyles.Strict)));
        Assert.That(sut.GetBrowserId(driver.Object, null), Is.EqualTo(expected));
    }

    [Test,AutoMoqData]
    public void GetBrowserIdShouldReturnACorrectIdWhenTheWebDriverDoesntHaveCapabilities(IWebDriver driver, BrowserIdFactory sut)
    {
        var expected = new BrowserId("chrome",
                                     "BarPlatform",
                                     new SemanticBrowserVersion(SemVersion.Parse("1.2.3", SemVersionStyles.Strict)));
        Assert.That(sut.GetBrowserId(driver, new ChromeOptions {PlatformName = "BarPlatform", BrowserVersion = "1.2.3"}), Is.EqualTo(expected));
    }

    [Test,AutoMoqData]
    public void GetBrowserIdShouldReturnAnUnknownBrowserIdWhenWeKnowNothingAboutTheWebDriver(IWebDriver driver, BrowserIdFactory sut)
    {
        var expected = new BrowserId("Unknown browser", "Unknown platform", MissingBrowserVersion.Instance);
        Assert.That(sut.GetBrowserId(driver, null), Is.EqualTo(expected));
    }
}