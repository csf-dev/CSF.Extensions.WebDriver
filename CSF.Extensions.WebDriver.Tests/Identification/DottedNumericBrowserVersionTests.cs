namespace CSF.Extensions.WebDriver.Identification;

[TestFixture,Parallelizable]
public class DottedNumericBrowserVersionTests
{
    [Test,AutoMoqData]
    public void ToStringShouldReturnAFormattedVersion()
    {
        var sut = new DottedNumericBrowserVersion([2,3,4]);
        Assert.That(sut.ToString(), Is.EqualTo("2.3.4"));
    }

    [Test,AutoMoqData]
    public void EqualsShouldReturnTrueForTwoEqualVersions()
    {
        var one = new DottedNumericBrowserVersion([2,3,4]);
        var two = new DottedNumericBrowserVersion([2,3,4]);
        Assert.That(one.Equals(two), Is.True);
    }

    [Test,AutoMoqData]
    public void EqualsShouldReturnFalseForTwoDifferentVersions()
    {
        var one = new DottedNumericBrowserVersion([2,3,4]);
        var two = new DottedNumericBrowserVersion([2,3,5]);
        Assert.That(one.Equals(two), Is.False);
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnANegativeForAHigherVersion()
    {
        var one = new DottedNumericBrowserVersion([2,3,4]);
        var two = new DottedNumericBrowserVersion([2,3,5]);
        Assert.That(one.CompareTo(two), Is.LessThan(0));
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnAPositiveForALowerVersion()
    {
        var one = new DottedNumericBrowserVersion([2,3,4]);
        var two = new DottedNumericBrowserVersion([2,3,3]);
        Assert.That(one.CompareTo(two), Is.GreaterThan(0));
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnZeroForAnEqualVersion()
    {
        var one = new DottedNumericBrowserVersion([2,3,4]);
        var two = new DottedNumericBrowserVersion([2,3,4]);
        Assert.That(one.CompareTo(two), Is.Zero);
    }

    [Test,AutoMoqData]
    public void TryParseShouldReturnTrueForAValidVersion()
    {
        Assert.That(DottedNumericBrowserVersion.TryParse("SomeKindOfPrefix3.4.5AVeryLongSuffix", out _), Is.True);
    }

    [Test,AutoMoqData]
    public void TryParseShouldReturnFalseForAnInvalidVersion()
    {
        Assert.That(DottedNumericBrowserVersion.TryParse("Elephants", out _), Is.False);
    }
}