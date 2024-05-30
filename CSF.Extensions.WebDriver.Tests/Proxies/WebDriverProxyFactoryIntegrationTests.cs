using CSF.Extensions.WebDriver.Identification;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OpenQA.Selenium;
using Semver;

namespace CSF.Extensions.WebDriver.Proxies;

[TestFixture, Parallelizable, Description("Integration tests for IGetsProxyWebDriver and all of its dependencies. These tests use DI.")]
public class WebDriverProxyFactoryIntegrationTests
{
    readonly IServiceProvider services;

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldGetAWebDriverWhichCanBeUnproxied(IWebDriver webDriver)
    {
        var sut = services.GetRequiredService<IGetsProxyWebDriver>();
        var proxy = sut.GetProxyWebDriver(webDriver, new ProxyCreationOptions());

        Assert.Multiple(() =>
        {
            Assert.That(proxy, Is.Not.SameAs(webDriver), "The proxy is a proxy and not the same as the original WebDriver");
            Assert.That(proxy.Unproxy(), Is.SameAs(webDriver), "Unproxying the proxy yields the original WebDriver");
        });
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichHasIdentificationWhenOptionsEnableIt(ICapabilities caps)
    {
        var webDriver = new Mock<IWebDriver>();
        webDriver.As<IHasCapabilities>().SetupGet(x => x.Capabilities).Returns(caps);
        Mock.Get(caps).Setup(x => x.GetCapability("browserName")).Returns("FooBrowser");
        Mock.Get(caps).Setup(x => x.GetCapability("platformName")).Returns("BarPlatform");
        Mock.Get(caps).Setup(x => x.GetCapability("browserVersion")).Returns("4.5.6");

        var sut = services.GetRequiredService<IGetsProxyWebDriver>();
        var proxy = sut.GetProxyWebDriver(webDriver.Object, new ProxyCreationOptions { AddIdentification = true });
        var expectedId = new BrowserId("FooBrowser", "BarPlatform", new SemanticBrowserVersion(SemVersion.Parse("4.5.6", SemVersionStyles.Strict)));

        Assert.That(() => ((IHasBrowserId)proxy).BrowserId, Is.EqualTo(expectedId));
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichDoesNotHaveIdentificationWhenOptionsDoNotEnableIt(ICapabilities caps)
    {
        var webDriver = new Mock<IWebDriver>();
        webDriver.As<IHasCapabilities>().SetupGet(x => x.Capabilities).Returns(caps);
        Mock.Get(caps).Setup(x => x.GetCapability("browserName")).Returns("FooBrowser");
        Mock.Get(caps).Setup(x => x.GetCapability("platformName")).Returns("BarPlatform");
        Mock.Get(caps).Setup(x => x.GetCapability("browserVersion")).Returns("4.5.6");

        var sut = services.GetRequiredService<IGetsProxyWebDriver>();
        var proxy = sut.GetProxyWebDriver(webDriver.Object, new ProxyCreationOptions());
        var expectedId = new BrowserId("FooBrowser", "BarPlatform", new SemanticBrowserVersion(SemVersion.Parse("4.5.6", SemVersionStyles.Strict)));

        Assert.That(proxy, Is.Not.InstanceOf<IHasBrowserId>());
    }
    
    public WebDriverProxyFactoryIntegrationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebDriverFactoryWithoutOptionsPattern();
        services = serviceCollection.BuildServiceProvider();
    }
}