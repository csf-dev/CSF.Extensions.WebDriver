using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverTypesProviderTests
{
    [Test,AutoMoqData]
    public void GetWebDriverTypeShouldReturnATypeForOneThatIsBuiltIntoSelenium([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                               WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverType(typeof(ChromeDriver).AssemblyQualifiedName), Is.EqualTo(typeof(ChromeDriver)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverTypeShouldReturnATypeForOneThatIsBuiltIntoSeleniumUsingAShortName([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                              WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverType(typeof(ChromeDriver).Name), Is.EqualTo(typeof(ChromeDriver)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverTypeShouldReturnATypeForAThirdPartyDriver([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                      WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverType(typeof(FakeWebDriver).AssemblyQualifiedName), Is.EqualTo(typeof(FakeWebDriver)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverTypeShouldThrowForANonsenseDriverType([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                  WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverType("NonsenseTypeName"), Throws.InstanceOf<TypeLoadException>());
    }

    [Test,AutoMoqData]
    public void GetWebDriverTypeShouldThrowForANullDriverType([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                  WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverType(null), Throws.ArgumentException);
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldReturnAnImpliedOptionsTypeForOneThatIsBuiltIntoSelenium([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                                     WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(ChromeDriver), null), Is.EqualTo(typeof(ChromeOptions)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldThrowIfTypeNameIsNullAndTheDriverDoesNotImplyAType([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                                WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(RemoteWebDriver), null), Throws.ArgumentException);
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldReturnATypeForAnExplitlySpecifiedTypeName([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                       WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(RemoteWebDriver), typeof(ChromeOptions).AssemblyQualifiedName), Is.EqualTo(typeof(ChromeOptions)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldReturnATypeForAShortTypeNameForATypeBuiltIntoSelenium([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                                   WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(RemoteWebDriver), typeof(ChromeOptions).Name), Is.EqualTo(typeof(ChromeOptions)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldReturnATypeForAThirdPartyOptionsTypeName([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                                      WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(RemoteWebDriver), typeof(FakeOptions).AssemblyQualifiedName), Is.EqualTo(typeof(FakeOptions)));
    }

    [Test,AutoMoqData]
    public void GetWebDriverOptionsTypeShouldThrowForANonsenseOptionsType([Frozen] IGetsWebDriverWithDeterministicOptionsTypes typesScanner,
                                                                          WebDriverTypesProvider sut)
    {
        Mock.Get(typesScanner)
            .Setup(x => x.GetWebDriverAndDeterministicOptionsTypes())
            .Returns([new(typeof(ChromeDriver), typeof(ChromeOptions))]);

        Assert.That(() => sut.GetWebDriverOptionsType(typeof(RemoteWebDriver), "NonsenseTypeName"), Throws.InstanceOf<TypeLoadException>());
    }

    public class FakeWebDriver : IWebDriver
    {
        public string Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Title => throw new NotImplementedException();

        public string PageSource => throw new NotImplementedException();

        public string CurrentWindowHandle => throw new NotImplementedException();

        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();

        public void Close() => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();

        public IWebElement FindElement(By by) => throw new NotImplementedException();

        public ReadOnlyCollection<IWebElement> FindElements(By by) => throw new NotImplementedException();

        public IOptions Manage() => throw new NotImplementedException();

        public INavigation Navigate() => throw new NotImplementedException();

        public void Quit() => throw new NotImplementedException();

        public ITargetLocator SwitchTo() => throw new NotImplementedException();
    }

    public class FakeOptions : DriverOptions
    {
        public override ICapabilities ToCapabilities() => throw new NotImplementedException();
    }
}