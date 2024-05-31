using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Quirks;
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

    
    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichHasQuirksFromStaticDataWhenOptionsEnableIt(ICapabilities caps)
    {
        var webDriver = new Mock<IWebDriver>();
        webDriver.As<IHasCapabilities>().SetupGet(x => x.Capabilities).Returns(caps);
        Mock.Get(caps).Setup(x => x.GetCapability("browserName")).Returns("FooBrowser");
        Mock.Get(caps).Setup(x => x.GetCapability("platformName")).Returns("BarPlatform");
        Mock.Get(caps).Setup(x => x.GetCapability("browserVersion")).Returns("2.3.4");

        var sut = services.GetRequiredService<IGetsProxyWebDriver>();
        var proxy = sut.GetProxyWebDriver(webDriver.Object, new ProxyCreationOptions { AddQuirks = true });

        Assert.Multiple(() =>
        {
            Assert.That(() => ((IHasQuirks)proxy).HasQuirk("SampleQuirk"), Is.True, "Browser has SampleQuirk");
            Assert.That(() => ((IHasQuirks)proxy).HasQuirk("OtherQuirk"), Is.False, "Browser does not have OtherQuirk");
        });
    }

    
    public WebDriverProxyFactoryIntegrationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddWebDriverFactoryWithoutOptionsPattern()
            .AddQuirksServices(new QuirksData { Quirks = new Dictionary<string, BrowserInfoCollection> {
                { "SampleQuirk", new() {
                    AffectedBrowsers = new HashSet<BrowserInfo>
                    {
                        new() { Name = "FooBrowser", MinVersion = "1.2.3", MaxVersion = "4.5.6" },
                    }
                }},
                { "OtherQuirk", new() {
                    AffectedBrowsers = new HashSet<BrowserInfo>
                    {
                        new() { Name = "FooBrowser", MinVersion = "4.5.6", MaxVersion = "7.8.9" },
                    }
                }},
            }});
        services = serviceCollection.BuildServiceProvider();
    }
}