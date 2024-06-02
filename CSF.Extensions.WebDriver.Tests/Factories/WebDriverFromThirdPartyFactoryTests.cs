using AutoFixture.NUnit3;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverFromThirdPartyFactoryTests
{
    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryFromAppropriateOptions([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                          [Frozen] IServiceProvider services,
                                                                                          WebDriverFromThirdPartyFactory sut,
                                                                                          RemoteWebDriverFromOptionsFactory remoteFactory)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => new FakeWebDriverFactory());
        Mock.Get(services).Setup(x => x.GetService(typeof(RemoteWebDriverFromOptionsFactory))).Returns(remoteFactory);
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));

        // This test should never throw because the fake factory should never execute any of Selenium's real logic
        using var driver = sut.GetWebDriver(options).WebDriver;
        Assert.That(driver, Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryFromDI([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                          [Frozen] IServiceProvider services,
                                                                          WebDriverFromThirdPartyFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => new FakeWebDriverFactory());
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));

        using var driver = sut.GetWebDriver(options).WebDriver;
        Mock.Get(services).Verify(x => x.GetService(typeof(FakeWebDriverFactory)), Times.Once);
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryWithoutDI([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                             [Frozen] IServiceProvider services,
                                                                             WebDriverFromThirdPartyFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => null);

        using var driver = sut.GetWebDriver(options).WebDriver;
        Assert.That(driver, Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldUseNextImplIfOptionsDoNotIndicateRemote([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                          [Frozen] ICreatesWebDriverFromOptions next,
                                                                          WebDriverFromThirdPartyFactory sut,
                                                                          IWebDriver driverFromNext)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = "SomeOtherDriver",
            OptionsFactory = () => new ChromeOptions(),
        };
        Mock.Get(next).Setup(x => x.GetWebDriver(options, null)).Returns(new WebDriverAndOptions(driverFromNext, new ChromeOptions()));

        using var driver = sut.GetWebDriver(options).WebDriver;
        Assert.That(driver, Is.SameAs(driverFromNext));
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCustomiseDriverOptionsWithCallbackWhenItIsSpecifiedWithACustomDriverFactory([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                                              [Frozen] IServiceProvider services,
                                                                                                              WebDriverFromThirdPartyFactory sut)
    {
        var driverOptions = new ChromeOptions();
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            OptionsFactory = () => driverOptions,
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => new FakeWebDriverFactory());
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));

        // This test should never throw because the fake factory should never execute any of Selenium's real logic
        using var driver = sut.GetWebDriver(options, o => o.AddAdditionalOption("Foo", "Bar")).WebDriver;

        Assert.That(driverOptions.ToCapabilities()["Foo"], Is.EqualTo("Bar"));
    }

    public class FakeWebDriverFactory : ICreatesWebDriverFromOptions
    {
        public WebDriverAndOptions GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions>? supplementaryConfiguration = null)
        {
            var driverOptions = options.OptionsFactory();
            supplementaryConfiguration?.Invoke(driverOptions);
            return new WebDriverAndOptions(Mock.Of<IWebDriver>(), driverOptions);
        }
    }
}