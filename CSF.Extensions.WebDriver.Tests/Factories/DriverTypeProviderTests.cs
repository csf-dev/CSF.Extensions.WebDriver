using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture, Parallelizable]
public class DriverTypeProviderTests
{
    [Test, AutoMoqData]
    public void TryGetDriverTypeShouldReturnTrueForAKnownType([StandardTypes] DriverTypeProvider sut,
                                                                    WebDriverCreationOptions options,
                                                                    IConfigurationSection config)
    {
        options.DriverType = nameof(ChromeDriver);
        Assert.That(sut.TryGetDriverType(options, config, out _), Is.True);
    }

    [Test, AutoMoqData]
    public void TryGetDriverTypeShouldReturnFalseForAnUnknownType([StandardTypes] DriverTypeProvider sut,
                                                                        WebDriverCreationOptions options,
                                                                        IConfigurationSection config)
    {
        options.DriverType = "Elephant";
        options.DriverFactoryType = null;
        Assert.That(sut.TryGetDriverType(options, config, out _), Is.False);
    }

    [Test, AutoMoqData]
    public void TryGetDriverTypeShouldReturnFalseIfDriverTypeIsNullAndDriverFactoryIsToo([StandardTypes] DriverTypeProvider sut,
                                                                                               WebDriverCreationOptions options,
                                                                                               IConfigurationSection config)
    {
        options.DriverType = null;
        options.DriverFactoryType = null;
        Assert.That(sut.TryGetDriverType(options, config, out _), Is.False);
    }

    [Test, AutoMoqData]
    public void TryGetDriverTypeShouldReturnTrueForAnUnknownTypeIfDriverFactoryIsNotNull([StandardTypes] DriverTypeProvider sut,
                                                                                               WebDriverCreationOptions options,
                                                                                               IConfigurationSection config,
                                                                                               string factoryType)
    {
        options.DriverType = "Elephant";
        options.DriverFactoryType = factoryType;
        Assert.That(sut.TryGetDriverType(options, config, out _), Is.True);
    }

    [Test, AutoMoqData]
    public void TryGetDriverTypeShouldReturnTrueIfDriverTypeIsNullIfDriverFactoryIsNotNull([StandardTypes] DriverTypeProvider sut,
                                                                                                 WebDriverCreationOptions options,
                                                                                                 IConfigurationSection config,
                                                                                                 string factoryType)
    {
        options.DriverType = null;
        options.DriverFactoryType = factoryType;
        Assert.That(sut.TryGetDriverType(options, config, out _), Is.True);
    }
}