using OpenQA.Selenium.Chrome;

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
            OptionsCustomizer = new AppveyorLinuxChromeCustomizer(),
        };

        using var driver = sut.GetWebDriver(options).WebDriver;
        Assert.That(driver, Is.Not.Null);
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
            OptionsCustomizer = new AppveyorLinuxChromeCustomizer(),
        };

        using var driver = sut.GetWebDriver(options, o => o.AddAdditionalOption("Foo", "Bar")).WebDriver;
        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }

    public class AppveyorLinuxChromeCustomizer : ICustomizesOptions<ChromeOptions>
    {
        public void CustomizeOptions(ChromeOptions options)
        {
            if(string.Equals(Environment.GetEnvironmentVariable("APPVEYOR"), bool.TrueString, StringComparison.InvariantCultureIgnoreCase)
               && Environment.GetEnvironmentVariable("APPVEYOR_BUILD_WORKER_IMAGE")!.Contains("ubuntu", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.Error.WriteLine("Running on Appveyor Linux, customising Chrome options.");
                options.BinaryLocation = "/usr/bin/google-chrome";
                options.AddArgument("--no-sandbox");
            }
        }
    }
}
