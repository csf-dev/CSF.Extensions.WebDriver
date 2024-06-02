using System.Reflection;
using AutoFixture.NUnit3;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverFromOptionsFactoryTests
{
    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateALocalChromeDriverFromAppropriateOptions([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                 WebDriverFromOptionsFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(ChromeDriver),
            OptionsFactory = () => new ChromeOptions(),
        };

        try
        {
            using var driver = sut.GetWebDriver(options).WebDriver;
            Assert.That(driver, Is.Not.Null);
        }
        catch (Exception e) when (e is TargetInvocationException { InnerException: DriverServiceNotFoundException } or DriverServiceNotFoundException)
        {
            Assert.Pass("Despite the exception raised, this is only because the driver isn't installed on the environment running the test; this is more than enough to prove that the driver was being created.");
        }
        
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCustomiseDriverOptionsWithCallbackWhenItIsSpecifiedWithALocalFactory([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                                       WebDriverFromOptionsFactory sut)
    {
        var driverOptions = new ChromeOptions();
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(ChromeDriver),
            OptionsFactory = () => driverOptions,
        };

        try
        {
            using var driver = sut.GetWebDriver(options, o => o.AddAdditionalOption("Foo", "Bar")).WebDriver;
        }
        catch (Exception e) when (e is TargetInvocationException { InnerException: DriverServiceNotFoundException } or DriverServiceNotFoundException)
        {
            // Intentionally ignore this exception; we know it's going to fail but I care only about how the options were manipulated in this test.
        }
                
        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }
}
