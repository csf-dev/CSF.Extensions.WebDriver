using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture, Parallelizable]
public class OptionsTypeProviderTests
{
    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnTrueForAKnownType([StandardTypes] OptionsTypeProvider sut,
                                                                    WebDriverCreationOptions options,
                                                                    IConfigurationSection config)
    {
        options.OptionsType = nameof(ChromeOptions);
        Assert.That(sut.TryGetOptionsType(options, config, typeof(ChromeDriver), out _), Is.True);
    }

    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnFalseForAnUnknownType([StandardTypes] OptionsTypeProvider sut,
                                                                        WebDriverCreationOptions options,
                                                                        IConfigurationSection config)
    {
        options.OptionsType = "Elephant";
        options.DriverFactoryType = null;
        Assert.That(sut.TryGetOptionsType(options, config, typeof(ChromeDriver), out _), Is.False);
    }

    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnTrueIfOptionsTypeIsNullButDriverIsAKnownType([StandardTypes] OptionsTypeProvider sut,
                                                                                               WebDriverCreationOptions options,
                                                                                               IConfigurationSection config)
    {
        options.OptionsType = null;
        options.DriverFactoryType = null;
        Assert.That(sut.TryGetOptionsType(options, config, typeof(ChromeDriver), out _), Is.True);
    }

    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnFalseIfOptionsTypeIsNullButDriverIsAnUnknownType([StandardTypes] OptionsTypeProvider sut,
                                                                                               WebDriverCreationOptions options,
                                                                                               IConfigurationSection config)
    {
        options.OptionsType = null;
        options.DriverFactoryType = null;
        Assert.That(sut.TryGetOptionsType(options, config, typeof(RemoteWebDriver), out _), Is.False);
    }

    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnTrueForAnUnknownTypeIfDriverFactoryTypeIsNotNull([StandardTypes] OptionsTypeProvider sut,
                                                                                               WebDriverCreationOptions options,
                                                                                               IConfigurationSection config,
                                                                                               string factoryType)
    {
        options.OptionsType = "Elephant";
        options.DriverFactoryType = factoryType;
        Assert.That(sut.TryGetOptionsType(options, config, typeof(ChromeDriver), out _), Is.True);
    }

    [Test, AutoMoqData]
    public void TryGetOptionsTypeShouldReturnTrueIfOptionsTypeIsNullIfDriverFactoryIsNotNull([StandardTypes] OptionsTypeProvider sut,
                                                                                                 WebDriverCreationOptions options,
                                                                                                 IConfigurationSection config,
                                                                                                 string factoryType)
    {
        options.OptionsType = null;
        options.DriverFactoryType = factoryType;
        Assert.That(sut.TryGetOptionsType(options, config, typeof(RemoteWebDriver), out _), Is.True);
    }
}