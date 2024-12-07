namespace CSF.Extensions.WebDriver.Factories;

[TestFixture,Parallelizable]
public class WebDriverFactoryExtensionsTests
{
    [Test,AutoMoqData]
    public void GetWebDriverShouldNotThrowIfSelectedConfigIsEmptyButOnlyOneConfigPresent(ICreatesWebDriverFromOptions factory,
                                                                                         WebDriverCreationOptions options,
                                                                                         WebDriverAndOptions expectedResult)
    {
        var configCollection = new WebDriverCreationOptionsCollection();
        configCollection.DriverConfigurations.Add("config", options);
        WebDriverAndOptions? result = null;
        Mock.Get(factory).Setup(x => x.GetWebDriver(options, null)).Returns(expectedResult);

        Assert.Multiple(() =>
        {
            Assert.That(() => result = factory.GetWebDriver(configCollection), Throws.Nothing, "Does not throw exception");
            Assert.That(result, Is.SameAs(expectedResult), "Correct expected result");
        });
    }
}