using System.Reflection;
using AutoFixture;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace CSF.Extensions.WebDriver.Factories;

/// <summary>
/// Attribute indicating that the type provider should return appropriate types for a few standard driver/options types,
/// shipped with Selenium.
/// </summary>
/// <remarks>
/// <para>
/// This also sets up the mocked implementation of <see cref="IGetsWebDriverAndOptionsTypes"/> to use strict mock behaviour.
/// This is because the signatures of its methods require that they either return a non-null type or throw an exception.
/// </para>
/// </remarks>
public class StandardTypesAttribute : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
        => new StandardTypesCustomization();
}

public class StandardTypesCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IGetsWebDriverAndOptionsTypes>(c => c.FromFactory(() => {
            var mock = new Mock<IGetsWebDriverAndOptionsTypes>(MockBehavior.Strict);
            mock.Setup(x => x.GetWebDriverType(nameof(ChromeDriver))).Returns(typeof(ChromeDriver));
            mock.Setup(x => x.GetWebDriverType(nameof(FirefoxDriver))).Returns(typeof(FirefoxDriver));
            mock.Setup(x => x.GetWebDriverType(nameof(RemoteWebDriver))).Returns(typeof(RemoteWebDriver));
            mock.Setup(x => x.GetWebDriverType(nameof(SafariDriver))).Returns(typeof(SafariDriver));
            mock.Setup(x => x.GetWebDriverOptionsType(typeof(ChromeDriver), null)).Returns(typeof(ChromeOptions));
            mock.Setup(x => x.GetWebDriverOptionsType(typeof(FirefoxDriver), null)).Returns(typeof(FirefoxOptions));
            mock.Setup(x => x.GetWebDriverOptionsType(typeof(SafariDriver), null)).Returns(typeof(SafariOptions));
            return mock.Object;
        }));
        fixture.Freeze<IGetsWebDriverAndOptionsTypes>();
    }
}