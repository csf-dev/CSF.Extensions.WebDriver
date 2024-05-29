using System.Collections.ObjectModel;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;
using Moq;
using OpenQA.Selenium;
using Semver;

namespace CSF.Extensions.WebDriver.Proxies;

[TestFixture,Parallelizable]
public class WebDriverProxyFactoryTests
{
    IGetsProxyWebDriver sut;

    [OneTimeSetUp]
    public void FixtureSetup()
    {
        sut = new WebDriverProxyFactory(new BrowserIdFactory(), new ProxyGenerator());
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldNotReturnNullWhenIdentificationIsEnabled(FakeWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationContext { AddIdentification = true }), Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldNotReturnNullWhenIdentificationIsDisabled(FakeWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationContext { AddIdentification = false }), Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichImplementsIHasUnproxiedWebDriver(FakeWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationContext()), Is.AssignableTo<IHasUnproxiedWebDriver>());
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichCanExposeTheUnproxiedDriver(FakeWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationContext()).Unproxy(), Is.SameAs(webDriver));
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichIsNotTheWebDriver(FakeWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationContext()), Is.Not.SameAs(webDriver));
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichHasIdentificationFromCapabilities(ICapabilities caps)
    {
        /* This test is rather profound.  It verifies that the proxy factory can create a proxy from an object which implements
         * multiple interfaces, in this case IWebDriver and IHasCapabilities.
         * It then exercises BrowserIdFactory to get a browser ID from the capabilities.
         * It adds an extra interface to the returned proxy: IHasBrowserId
         * Finally it verifies that when using the proxy via that extra interface, it returns the correct value as provided by the interceptor.
         */

        var webDriverMock = new Mock<IWebDriver>();
        webDriverMock.As<IHasCapabilities>().SetupGet(x => x.Capabilities).Returns(caps);
        Mock.Get(caps).Setup(x => x.GetCapability("browserName")).Returns("FooBrowser");
        Mock.Get(caps).Setup(x => x.GetCapability("platformName")).Returns("BarPlatform");
        Mock.Get(caps).Setup(x => x.GetCapability("browserVersion")).Returns("4.5.6");

        var proxy = sut.GetProxyWebDriver(webDriverMock.Object, new ProxyCreationContext { AddIdentification = true });
        var expectedId = new BrowserId("FooBrowser", "BarPlatform", new SemanticBrowserVersion(SemVersion.Parse("4.5.6", SemVersionStyles.Strict)));
        Assert.That(() => {
            var browserIdProxy = (IHasBrowserId)proxy;
            return browserIdProxy.BrowserId;
        }, Is.EqualTo(expectedId));
    }

    public class FakeWebDriver : IWebDriver
    {
        public string Url { get; set; } = "https://example.com";

        public string Title => throw new NotImplementedException();

        public string PageSource => throw new NotImplementedException();

        public string CurrentWindowHandle => throw new NotImplementedException();

        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();

        public void Close() => throw new NotImplementedException();

        public void Dispose() {}

        public IWebElement FindElement(By by) => throw new NotImplementedException();

        public ReadOnlyCollection<IWebElement> FindElements(By by) => throw new NotImplementedException();

        public IOptions Manage() => throw new NotImplementedException();

        public INavigation Navigate() => throw new NotImplementedException();

        public void Quit() => throw new NotImplementedException();

        public ITargetLocator SwitchTo() => throw new NotImplementedException();
    }
}

