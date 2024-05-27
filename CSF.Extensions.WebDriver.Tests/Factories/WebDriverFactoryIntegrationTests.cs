using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverFactoryIntegrationTests
{
    [Test]
    public void GetDefaultWebDriverShouldThrowAnExceptionBecauseOfAnInvalidUrlScheme()
    {
        var services = GetServiceProvider();
        var driverFactory = services.GetRequiredService<IGetsWebDriver>();
        Assert.That(() => driverFactory.GetDefaultWebDriver(), Throws.InstanceOf<NotSupportedException>().And.Message.Contains("'invalid'"));
    }

    [Test]
    public void GetDefaultWebDriverAfterConfigurationCallbackShouldThrowAnExceptionBecauseOfANonsenseUrlScheme()
    {
        var services = GetServiceProvider(o => o.SelectedConfiguration = "DefaultNonsense");
        var driverFactory = services.GetRequiredService<IGetsWebDriver>();
        Assert.That(() => driverFactory.GetDefaultWebDriver(), Throws.InstanceOf<NotSupportedException>().And.Message.Contains("'nonsense'"));
    }

    IServiceProvider GetServiceProvider(Action<WebDriverCreationOptionsCollection>? configureOptions = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(GetConfiguration());
        services.AddWebDriverFactory(configureOptions: configureOptions);
        services.AddLogging();
        return services.BuildServiceProvider();
    }

    IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.WebDriverFactoryIntegrationTests.json")
            .Build();
    }
}