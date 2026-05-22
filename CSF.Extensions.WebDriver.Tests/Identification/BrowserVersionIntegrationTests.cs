namespace CSF.Extensions.WebDriver.Identification;

[TestFixture, Parallelizable]
public class BrowserVersionIntegrationTests
{
    [Test]
    public void ALowerVersionShouldBeLessThanAHigherOneWithMoreComponents()
    {
        var first = BrowserVersion.Create("95.0.4638");
        var second = BrowserVersion.Create("95.1.1234.5678");

        Assert.That(first, Is.LessThan(second), "First version is less than second version");
    }

    [Test]
    public void AHigherVersionShouldBeGreaterThanALowerOneWithMoreComponents()
    {
        var first = BrowserVersion.Create("95.2.4638");
        var second = BrowserVersion.Create("95.1.1234.5678");

        Assert.That(first, Is.GreaterThan(second), "First version is greater than second version");
    }

    [Test]
    public void ALowerVersionShouldBeLessThanAHigherOneWithFewerComponents()
    {
        var first = BrowserVersion.Create("95.0.4638.1234");
        var second = BrowserVersion.Create("95.1");

        Assert.That(first, Is.LessThan(second), "First version is less than second version");
    }

    [Test]
    public void AHigherVersionShouldBeGreaterThanALowerOneWithFewerComponents()
    {
        var first = BrowserVersion.Create("95.2.4638.1234");
        var second = BrowserVersion.Create("95.1");

        Assert.That(first, Is.GreaterThan(second), "First version is greater than second version");
    }
}