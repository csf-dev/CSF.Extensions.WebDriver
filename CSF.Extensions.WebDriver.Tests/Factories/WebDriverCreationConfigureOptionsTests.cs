using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverCreationConfigureOptionsTests
{
    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToSetupLocalChromeDriverWithSimpleOptionsFromJsonConfiguration([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                                          [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": {
            ""DriverType"": ""ChromeDriver"",
            ""Options"": {
                ""BinaryLocation"": ""C:\\SomePath\\Chrome\\GoogleChrome.exe""
            }
        }
    }
}");

        Assert.Multiple(() =>
        {
            Assert.That(options.DriverConfigurations, Has.Count.EqualTo(1), "Count of driver configurations should be 1");
            Assert.That(options.DriverConfigurations, Contains.Key("Test"), "Options contains item with correct key");
            var hasDriverConfig = options.DriverConfigurations.TryGetValue("Test", out var driverConfig);
            if(!hasDriverConfig)
            {
                Assert.Fail("The driver configuration should be present");
                return;
            }
            Assert.That(driverConfig?.DriverType, Is.EqualTo("ChromeDriver"), "Driver config should have correct driver type name");
            Assert.That(driverConfig?.OptionsFactory(), Is.InstanceOf<ChromeOptions>(), "Driver config should use correct options type");
#pragma warning disable NUnit2022 // Missing property required for constraint: Really, we are using the subclass ChromeOptions, which does have that property
            Assert.That(driverConfig?.OptionsFactory(),
                        Has.Property(nameof(ChromeOptions.BinaryLocation)).EqualTo(@"C:\SomePath\Chrome\GoogleChrome.exe"),
                        "Driver config has options with correct binary location");
#pragma warning restore NUnit2022 // Missing property required for constraint
        });
    }


    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToSetupTwoLocalDriversWithSimpleOptionsFromJsonConfiguration([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                                        [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""SampleChrome"": {
            ""DriverType"": ""ChromeDriver"",
            ""Options"": {
                ""BinaryLocation"": ""C:\\SomePath\\Chrome\\GoogleChrome.exe""
            }
        },
        ""SampleFirefox"": {
            ""DriverType"": ""FirefoxDriver"",
            ""Options"": {
                ""EnableDevToolsProtocol"": true
            }
        },
    }
}");

        Assert.Multiple(() =>
        {
            Assert.That(options.DriverConfigurations, Has.Count.EqualTo(2), "Count of driver configurations should be 2");
            Assert.That(options.DriverConfigurations, Contains.Key("SampleChrome"), "Options contains item for Chrome");
            Assert.That(options.DriverConfigurations, Contains.Key("SampleFirefox"), "Options contains item for Firefox");
            var hasDriverConfig = options.DriverConfigurations.TryGetValue("SampleFirefox", out var driverConfig);
            if(!hasDriverConfig)
            {
                Assert.Fail("The driver configuration for Firefox should be present");
                return;
            }
#pragma warning disable NUnit2022 // Missing property required for constraint: Really, we are using the subclass FirefoxOptions, which does have that property
            Assert.That(driverConfig?.OptionsFactory(),
                        Has.Property(nameof(FirefoxOptions.EnableDevToolsProtocol)).True,
                        "Driver config has options with correct dev tools protocol setting");
#pragma warning restore NUnit2022 // Missing property required for constraint
        });
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToSetupLocalChromeDriverWithNoOptionsFromJsonConfiguration([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                                      [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""ChromeDriver"" }
    }
}");

        Assert.Multiple(() =>
        {
            var hasDriverConfig = options.DriverConfigurations.TryGetValue("Test", out var driverConfig);
            if(!hasDriverConfig)
            {
                Assert.Fail("The driver configuration should be present");
                return;
            }
            Assert.That(driverConfig?.OptionsFactory(), Is.InstanceOf<ChromeOptions>(), "Driver config should use correct options type");
            Assert.That(driverConfig?.OptionsFactory(), Is.Not.Null, "Driver config should not be null");
        });
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToGetSelectedConfigWhenThereIsOnlyOnePresent([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                        [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""ChromeDriver"" }
    }
}");

        Assert.That(options.GetSelectedConfiguration()?.DriverType, Is.EqualTo("ChromeDriver"));
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToAddACustomizerToSomeOptions([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                         [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""ChromeDriver"", ""OptionsCustomizerType"": ""CSF.Extensions.WebDriver.Factories.SampleCustomizer, CSF.Extensions.WebDriver.Tests"" }
    }
}");

        Assert.That(options.GetSelectedConfiguration()?.OptionsCustomizer, Is.InstanceOf<SampleCustomizer>());
    }
    
    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToGetSelectedConfigWhenASelectedConfigIsNamed([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                         [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""ChromeDriver"" },
        ""Test2"": { ""DriverType"": ""FirefoxDriver"" }
    },
    ""SelectedConfiguration"": ""Test2""
}");

        Assert.That(options.GetSelectedConfiguration()?.DriverType, Is.EqualTo("FirefoxDriver"));
    }
    
    [Test,AutoMoqData]
    public async Task ConfigureShouldProvideThrowWhenThereAreTwoConfigsAndNoExplicitSelection([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                              [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""ChromeDriver"" },
        ""Test2"": { ""DriverType"": ""FirefoxDriver"" }
    }
}");

        Assert.That(() => options.GetSelectedConfiguration(), Throws.InvalidOperationException);
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldNotThrowForANonsenseDriverType([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                    [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""NonexistentDriver"" }
    }
}");

        Assert.That(options.DriverConfigurations, Is.Empty);
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldNotThrowForARemoteDriverWithoutOptionsType([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                [StandardTypes] IGetsOptionsType optionsTypeProvider)
    {
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""RemoteWebDriver"" }
    }
}");

        Assert.That(options.DriverConfigurations, Is.Empty);
    }

    [Test,AutoMoqData]
    public async Task ConfigureShouldBeAbleToSetupRemoteDriverWithSimpleOptionsFromJsonConfiguration([StandardTypes] IGetsDriverType driverTypeProvider,
                                                                                                     [StandardTypes] IGetsOptionsType optionsTypeProvider,
                                                                                                     [TestLogger] ILogger<WebDriverCreationConfigureOptions> logger)
    {
        var type = typeof(SafariOptions);
        Mock.Get(optionsTypeProvider)
            .Setup(x => x.TryGetOptionsType(It.Is<WebDriverCreationOptions>(o => o.OptionsType == nameof(SafariOptions)), It.IsAny<IConfigurationSection>(), typeof(RemoteWebDriver), out type))
            .Returns(true);
        var options = await GetOptionsAsync(driverTypeProvider, optionsTypeProvider,
@"{
    ""DriverConfigurations"": {
        ""Test"": { ""DriverType"": ""RemoteWebDriver"", ""OptionsType"": ""SafariOptions"" }
    }
}", logger: logger);

        Assert.That(options.DriverConfigurations, Is.Not.Empty);
    }


    /// <summary>
    /// Creates and exercises <see cref="WebDriverCreationConfigureOptions"/> in order to create a new
    /// <see cref="WebDriverCreationOptionsCollection"/> from a specified JSON config.
    /// </summary>
    /// <param name="typeProvider">The type provider for web driver and options types</param>
    /// <param name="json">The JSON config from which to create the options</param>
    /// <returns>A task exposing the webdriver creation options collection, configured by the SUT</returns>
    static async Task<WebDriverCreationOptionsCollection> GetOptionsAsync(IGetsDriverType driverTypeProvider,
                                                                          IGetsOptionsType optionsTypeProvider,
                                                                          string json,
                                                                          ILogger<WebDriverCreationConfigureOptions>? logger = null)
    {
        var options = new WebDriverCreationOptionsCollection();
        var config = await ConfigurationFactory.GetConfigurationAsync(json);
        ICreatesDriverOptions optionsFactory = new ActivatorDriverOptionsFactory();
        optionsFactory = new ConfigBindingDriverOptionsFactoryDecorator(optionsFactory);
        var parser = new WebDriverConfigurationItemParser(driverTypeProvider, optionsTypeProvider, optionsFactory, Mock.Of<ILogger<WebDriverConfigurationItemParser>>());
        var sut = new WebDriverCreationConfigureOptions(parser,
                                                        config,
                                                        logger ?? Mock.Of<ILogger<WebDriverCreationConfigureOptions>>(),
                                                        c => {});
        sut.Configure(options);
        return options;
    }
}