using Microsoft.Extensions.Configuration;
using NUnit.Framework.Internal;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture, Parallelizable]
public class LogLevelDriverOptionsFactoryDecoratorTests
{
    [Test, AutoMoqData]
    public void CreateOptionsShouldSetLogLevelIfSpecifiedInConfig([Frozen] ICreatesDriverOptions wrapped,
                                                                  LogLevelDriverOptionsFactoryDecorator sut,
                                                                  Type optionsType,
                                                                  IConfigurationSection config)
    {
        var options = new InspectableDriverOptions();
        Mock.Get(wrapped).Setup(x => x.CreateOptions(optionsType, config)).Returns(options);
        Mock.Get(config)
            .Setup(x => x.GetSection(nameof(WebDriverCreationOptions.BrowserLogLevel)))
            .Returns(Mock.Of<IConfigurationSection>(x => x.Value == LogLevel.Severe.ToString()));

        sut.CreateOptions(optionsType, config);

        Assert.That(() => options.GetLoggingPrefs()[LogType.Browser], Is.EqualTo("SEVERE"));
    }

    [Test, AutoMoqData]
    public void CreateOptionsShouldNotSetLogLevelIfOmittedInConfig([Frozen] ICreatesDriverOptions wrapped,
                                                                   LogLevelDriverOptionsFactoryDecorator sut,
                                                                   Type optionsType,
                                                                   IConfigurationSection config)
    {
        var options = new InspectableDriverOptions();
        Mock.Get(wrapped).Setup(x => x.CreateOptions(optionsType, config)).Returns(options);
        Mock.Get(config)
            .Setup(x => x.GetSection(nameof(WebDriverCreationOptions.BrowserLogLevel)))
            .Returns(Mock.Of<IConfigurationSection>(x => x.Value == null));
            
        sut.CreateOptions(optionsType, config);

        Assert.That(() => options.GetLoggingPrefs(), Is.Null);
    }

    [Test, AutoMoqData]
    public void CreateOptionsShouldNotSetLogLevelIfConfigIsInvalid([Frozen] ICreatesDriverOptions wrapped,
                                                                   LogLevelDriverOptionsFactoryDecorator sut,
                                                                   Type optionsType,
                                                                   IConfigurationSection config)
    {
        var options = new InspectableDriverOptions();
        Mock.Get(wrapped).Setup(x => x.CreateOptions(optionsType, config)).Returns(options);
        Mock.Get(config)
            .Setup(x => x.GetSection(nameof(WebDriverCreationOptions.BrowserLogLevel)))
            .Returns(Mock.Of<IConfigurationSection>(x => x.Value == "invalid level"));
            
        sut.CreateOptions(optionsType, config);

        Assert.That(() => options.GetLoggingPrefs(), Is.Null);
    }

    class InspectableDriverOptions : DriverOptions
    {
        public Dictionary<string, object> GetLoggingPrefs() => GenerateLoggingPreferencesDictionary();

        public override ICapabilities ToCapabilities() => throw new NotImplementedException();
    }
}