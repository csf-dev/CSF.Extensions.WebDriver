using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable][Ignore("Temporarily ignored to diagnose #47")]
public class RemoteWebDriverFromOptionsFactoryTests
{
    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateARemoteDriverFromAppropriateOptions([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                            RemoteWebDriverFromOptionsFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
        };

        try
        {
            using var driver = sut.GetWebDriver(options).WebDriver;
            Assert.That(driver, Is.Not.Null);
        }
        catch (NotSupportedException)
        {
            Assert.Pass("This exception is expected because of the use of the 'nonsense' URL scheme to ensure that no real HTTP request is sent.");
        }
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldUseNextImplIfOptionsDoNotIndicateRemote([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                          [Frozen] ICreatesWebDriverFromOptions next,
                                                                          RemoteWebDriverFromOptionsFactory sut,
                                                                          IWebDriver driverFromNext)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = "ChromeDriver",
            OptionsFactory = () => new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
        };
        Mock.Get(next).Setup(x => x.GetWebDriver(options, null)).Returns(new WebDriverAndOptions(driverFromNext, new ChromeOptions()));

        using var driver = sut.GetWebDriver(options).WebDriver;
        Assert.That(driver, Is.SameAs(driverFromNext));
    }
    
    [Test,AutoMoqData]
    public void GetWebDriverShouldCustomiseDriverOptionsWithCallbackWhenItIsSpecifiedWithARemoteFactory([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                                        RemoteWebDriverFromOptionsFactory sut)
    {
        var driverOptions = new ChromeOptions();
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => driverOptions,
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
        };

        try
        {
            using var driver = sut.GetWebDriver(options, o => o.AddAdditionalOption("Foo", "Bar")).WebDriver;
        }
        catch (NotSupportedException)
        {
            // Intentionally ignore this exception; we know it's going to fail but I care only about how the options were manipulated in this test.
        }
        
        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }
}