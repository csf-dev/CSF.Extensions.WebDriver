using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture, Parallelizable]
public class SeleniumDriverAndOptionsScannerTests
{
    [Test,AutoMoqData]
    public void GetKnownDriverAndOptionsTypesShouldIncludeChromeDriverAndChromeOptions(SeleniumDriverAndOptionsScanner sut)
    {
        Assert.That(() => sut.GetWebDriverAndDeterministicOptionsTypes(), Has.One.EqualTo(new WebDriverAndOptionsTypePair(typeof(ChromeDriver), typeof(ChromeOptions))));
    }

    [Test,AutoMoqData]
    public void GetKnownDriverAndOptionsTypesShouldIncludeRemoteDriver(SeleniumDriverAndOptionsScanner sut)
    {
        Assert.That(() => sut.GetWebDriverAndDeterministicOptionsTypes(), Has.None.Matches<WebDriverAndOptionsTypePair>(wdo => wdo.WebDriverType == typeof(RemoteWebDriver)));
    }

    [Test,AutoMoqData,Description("At the time of writing, a specific set of drivers is supported by Selenium. This test asserts that they are all still supported; if Selenium introduces a breaking change such as removing support for a browser then this test will begin warning.")]
    public void GetKnownDriverAndOptionsTypesShouldContainAllOfTheExpectedDrivers(SeleniumDriverAndOptionsScanner sut)
    {
        var supportedDriverNames = sut.GetWebDriverAndDeterministicOptionsTypes().Select(x => x.WebDriverType.Name);
        var expectedDriverNames = new[]
        {
            "SafariDriver",
            "OperaDriver",
            "InternetExplorerDriver",
            "FirefoxDriver",
            "EdgeDriver",
            "ChromeDriver",
        };

        // This isn't strictly an assertion or a true "test", because maybe Selenium genuinely wish to drop support for a browser.
        // If they do then that's fine.
        // But - this will help point that fact out should it occur.
        Warn.Unless(supportedDriverNames,
                    Is.SupersetOf(expectedDriverNames),
                    "The collection of supported drivers no longer contains all of expected drivers; breaking change in Selenium?");
        Assert.Pass("This isn't a true test, but if there has been a warning emitted then perhaps Selenium has introduced a change in built-in browser support.");
    }
}