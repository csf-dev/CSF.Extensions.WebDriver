using System.Reflection;
using AutoFixture.NUnit3;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

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
            Options = new ChromeOptions(),
        };

        try
        {
            using var driver = sut.GetWebDriver(options);
            Assert.That(driver, Is.Not.Null);
        }
        catch (Exception e) when (e is TargetInvocationException { InnerException: DriverServiceNotFoundException } or DriverServiceNotFoundException)
        {
            Assert.Pass("Despite the exception raised, this is only because the driver isn't installed on the environment running the test; this is more than enough to prove that the driver was being created.");
        }
        
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateARemoteDriverFromAppropriateOptions([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                            [Frozen] IServiceProvider services,
                                                                            WebDriverFromOptionsFactory sut,
                                                                            RemoteWebDriverFromOptionsFactory remoteFactory)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            Options = new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(RemoteWebDriverFromOptionsFactory))).Returns(remoteFactory);

        try
        {
            using var driver = sut.GetWebDriver(options);
            Assert.That(driver, Is.Not.Null);
        }
        catch (NotSupportedException)
        {
            Assert.Pass("This exception is expected because of the use of the 'nonsense' URL scheme to ensure that no real HTTP request is sent.");
        }
        
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryFromAppropriateOptions([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                                          [Frozen] IServiceProvider services,
                                                                                          WebDriverFromOptionsFactory sut,
                                                                                          RemoteWebDriverFromOptionsFactory remoteFactory)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            Options = new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => new FakeWebDriverFactory());
        Mock.Get(services).Setup(x => x.GetService(typeof(RemoteWebDriverFromOptionsFactory))).Returns(remoteFactory);
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));

        // This test should never throw because the fake factory should never execute any of Selenium's real logic
        using var driver = sut.GetWebDriver(options);
        Assert.That(driver, Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryFromDI([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                          [Frozen] IServiceProvider services,
                                                                          WebDriverFromOptionsFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            Options = new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => new FakeWebDriverFactory());
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));

        using var driver = sut.GetWebDriver(options);
        Mock.Get(services).Verify(x => x.GetService(typeof(FakeWebDriverFactory)), Times.Once);
    }

    [Test,AutoMoqData]
    public void GetWebDriverShouldCreateAWebDriverFromCustomFactoryWithoutDI([StandardTypes] IGetsWebDriverAndOptionsTypes typeProvider,
                                                                             [Frozen] IServiceProvider services,
                                                                             WebDriverFromOptionsFactory sut)
    {
        var options = new WebDriverCreationOptions
        {
            DriverType = nameof(RemoteWebDriver),
            Options = new ChromeOptions(),
            GridUrl = "nonsense://127.0.0.1:1/nonexistent/path",
            DriverFactoryType = typeof(FakeWebDriverFactory).AssemblyQualifiedName,
        };
        Mock.Get(typeProvider).Setup(x => x.GetWebDriverFactoryType(typeof(FakeWebDriverFactory).AssemblyQualifiedName)).Returns(typeof(FakeWebDriverFactory));
        Mock.Get(services).Setup(x => x.GetService(typeof(FakeWebDriverFactory))).Returns(() => null);

        using var driver = sut.GetWebDriver(options);
        Assert.That(driver, Is.Not.Null);
    }

    public class FakeWebDriverFactory : ICreatesWebDriverFromOptions
    {
        public IWebDriver GetWebDriver(WebDriverCreationOptions options) => Mock.Of<IWebDriver>();
    }
}
