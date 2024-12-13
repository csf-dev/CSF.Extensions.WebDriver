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
        };

        using var driver = sut.GetWebDriver(options, o => o.AddAdditionalOption("Foo", "Bar")).WebDriver;
        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCustomiseDriverFromCustomizerInstanceIfSpecified([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                    WebDriverFromOptionsFactory sut)
    {
        var driverOptions = new ChromeOptions();
        var customizer = new AppveyorLinuxChromeCustomizer();
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(ChromeDriver),
            OptionsFactory = () => driverOptions,
            OptionsCustomizer = customizer,
        };

        using var driver = sut.GetWebDriver(options);
        Assert.That(customizer.IsCustomized, Is.True);
    }

    public class AppveyorLinuxChromeCustomizer : ICustomizesOptions<ChromeOptions>
    {
        public bool IsCustomized { get; private set; }

        public void CustomizeOptions(ChromeOptions options)
        {
            IsCustomized = true;
        }
    }
}
