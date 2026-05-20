using System.Reflection;
using AutoFixture;
using Microsoft.Extensions.Configuration;
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
    static readonly Type
        chromeDriverType = typeof(ChromeDriver),
        firefoxDriverType = typeof(FirefoxDriver),
        remoteDriverType = typeof(RemoteWebDriver),
        safariDriverType = typeof(SafariDriver),
        chromeOptionsType = typeof(ChromeOptions),
        firefoxOptionsType = typeof(FirefoxOptions),
        safariOptionsType = typeof(SafariOptions);

    public void Customize(IFixture fixture)
    {
        CustomizeDriverAndOptionsTypeProvider(fixture);
        CustomizeOptionsTypeProvider(fixture);
        CustomizeDriverType(fixture);
    }

    static void CustomizeDriverAndOptionsTypeProvider(IFixture fixture)
    {
        fixture.Customize<IGetsWebDriverAndOptionsTypes>(c => c.FromFactory(() => {
            var mock = new Mock<IGetsWebDriverAndOptionsTypes>(MockBehavior.Strict);
            mock.Setup(x => x.GetWebDriverType(nameof(ChromeDriver))).Returns(chromeDriverType);
            mock.Setup(x => x.GetWebDriverType(nameof(FirefoxDriver))).Returns(firefoxDriverType);
            mock.Setup(x => x.GetWebDriverType(nameof(RemoteWebDriver))).Returns(remoteDriverType);
            mock.Setup(x => x.GetWebDriverType(nameof(SafariDriver))).Returns(safariDriverType);
            mock.Setup(x => x.GetWebDriverOptionsType(chromeDriverType, null)).Returns(chromeOptionsType);
            mock.Setup(x => x.GetWebDriverOptionsType(firefoxDriverType, null)).Returns(firefoxOptionsType);
            mock.Setup(x => x.GetWebDriverOptionsType(safariDriverType, null)).Returns(safariOptionsType);
            return mock.Object;
        }));

        fixture.Freeze<IGetsWebDriverAndOptionsTypes>();
    }

    static void CustomizeOptionsTypeProvider(IFixture fixture)
    {
        fixture.Customize<IGetsOptionsType>(c => c.FromFactory(() =>
        {
            var mock = new Mock<IGetsOptionsType>(MockBehavior.Strict);
            Type nullType = null!;
            mock
                .Setup(x => x.TryGetOptionsType(It.IsAny<WebDriverCreationOptions>(), It.IsAny<IConfigurationSection>(), It.IsAny<Type>(), out nullType))
                .Returns(false);
            var chromeType = chromeOptionsType;
            mock
                .Setup(x => x.TryGetOptionsType(It.IsAny<WebDriverCreationOptions>(), It.IsAny<IConfigurationSection>(), chromeDriverType, out chromeType))
                .Returns(true);
            var firefoxType = firefoxOptionsType;
            mock
                .Setup(x => x.TryGetOptionsType(It.IsAny<WebDriverCreationOptions>(), It.IsAny<IConfigurationSection>(), firefoxDriverType, out firefoxType))
                .Returns(true);
            var safariType = safariOptionsType;
            mock
                .Setup(x => x.TryGetOptionsType(It.IsAny<WebDriverCreationOptions>(), It.IsAny<IConfigurationSection>(), safariDriverType, out safariType))
                .Returns(true);
            return mock.Object;
        }));

        fixture.Freeze<IGetsOptionsType>();
    }

    static void CustomizeDriverType(IFixture fixture)
    {
        fixture.Customize<IGetsDriverType>(c => c.FromFactory(() =>
        {
            var mock = new Mock<IGetsDriverType>(MockBehavior.Strict);
            Type nullType = null!;
            mock
                .Setup(x => x.TryGetDriverType(It.IsAny<WebDriverCreationOptions>(), It.IsAny<IConfigurationSection>(), out nullType))
                .Returns(false);
            var chromeType = chromeDriverType;
            mock
                .Setup(x => x.TryGetDriverType(It.Is<WebDriverCreationOptions>(o => o.DriverType == nameof(ChromeDriver)), It.IsAny<IConfigurationSection>(), out chromeType))
                .Returns(true);
            var firefoxType = firefoxDriverType;
            mock
                .Setup(x => x.TryGetDriverType(It.Is<WebDriverCreationOptions>(o => o.DriverType == nameof(FirefoxDriver)), It.IsAny<IConfigurationSection>(), out firefoxType))
                .Returns(true);
            var remoteType = remoteDriverType;
            mock
                .Setup(x => x.TryGetDriverType(It.Is<WebDriverCreationOptions>(o => o.DriverType == nameof(RemoteWebDriver)), It.IsAny<IConfigurationSection>(), out remoteType))
                .Returns(true);
            var safariType = safariDriverType;
            mock
                .Setup(x => x.TryGetDriverType(It.Is<WebDriverCreationOptions>(o => o.DriverType == nameof(SafariDriver)), It.IsAny<IConfigurationSection>(), out safariType))
                .Returns(true);

            return mock.Object;
        }));
        
        fixture.Freeze<IGetsDriverType>();
    }
}