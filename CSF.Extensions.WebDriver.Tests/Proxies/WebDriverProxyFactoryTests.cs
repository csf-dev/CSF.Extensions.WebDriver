using Moq;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies;

[TestFixture,Parallelizable]
public class WebDriverProxyFactoryTests
{
    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldNotReturnNull([ProxyFactory] WebDriverProxyFactory sut,
                                                     IWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationOptions()), Is.Not.Null);
    }

    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldApplyEachAugmentation([ProxyFactory] WebDriverProxyFactory sut, IWebDriver webDriver, IAugmentsProxyContext augmenter)
    {
        sut.GetProxyWebDriver(webDriver, new ProxyCreationOptions());
        Mock.Get(augmenter).Verify(x => x.AugmentContext(It.IsAny<WebDriverProxyCreationContext>()), Times.AtLeast(3));
    }


    [Test,AutoMoqData]
    public void GetProxyWebDriverShouldReturnAnObjectWhichIsNotTheWebDriver([ProxyFactory] WebDriverProxyFactory sut, IWebDriver webDriver)
    {
        Assert.That(() => sut.GetProxyWebDriver(webDriver, new ProxyCreationOptions()), Is.Not.SameAs(webDriver));
    }
}

