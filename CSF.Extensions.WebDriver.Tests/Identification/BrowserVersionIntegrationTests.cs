namespace CSF.Extensions.WebDriver.Identification;

[TestFixture, Parallelizable]
public class BrowserVersionIntegrationTests
{
    [Test]
    public void AHigherVersionShouldBeGreaterThanALowerOneWithMoreComponents()
    {
        var first = BrowserVersion.Create("95.0.4638.54");
        var second = BrowserVersion.Create("95.1");

        Assert.That(first, Is.LessThan(second), "First result is less than second result");
    }    
}