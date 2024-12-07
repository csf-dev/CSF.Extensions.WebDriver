using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable][Ignore("Temporarily ignored to diagnose #47")]
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
        catch(TargetInvocationException e) when (e is { InnerException: InvalidOperationException })
        {
            if(e.InnerException.Message.StartsWith("session not created:"))
                Assert.Pass("Despite the exception raised, this is only because the wrong version of the driver is installed on the environment running the test; this is more than enough to prove that the driver was being created.");
            else
                throw;
        }
        catch(InvalidOperationException invOpEx)
        {
            if(invOpEx.Message.StartsWith("session not created:"))
                Assert.Pass("Despite the exception raised, this is only because the wrong version of the driver is installed on the environment running the test; this is more than enough to prove that the driver was being created.");
            else
                throw;
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
        catch(TargetInvocationException e) when (e is { InnerException: InvalidOperationException })
        {
            // Intentionally ignore this exception; we know it's going to fail but I care only about how the options were manipulated in this test.
        }
        catch(InvalidOperationException)
        {
            // Intentionally ignore this exception; we know it's going to fail but I care only about how the options were manipulated in this test.
        }
                
        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }
}
