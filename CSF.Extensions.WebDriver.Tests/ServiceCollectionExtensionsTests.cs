using CSF.Extensions.WebDriver.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace CSF.Extensions.WebDriver;

[TestFixture,Parallelizable]
public class ServiceCollectionExtensionsTests
{
    [Test,AutoMoqData]
    public void AddWebDriverFactoryWithoutOptionsPatternShouldNotCrashWhenResolvingBecauseOfMissingLogging()
    {
        var services = new ServiceCollection();
        services.AddWebDriverFactoryWithoutOptionsPattern();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetRequiredService<ICreatesWebDriverFromOptions>, Throws.Nothing);
    }
}