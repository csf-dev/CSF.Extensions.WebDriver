using Semver;

namespace CSF.Extensions.WebDriver.Identification;

[TestFixture,Parallelizable]
public class SemanticBrowserVersionTests
{
    [Test,AutoMoqData]
    public void ToStringShouldReturnASemanticVersion()
    {
        var sut = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        Assert.That(sut.ToString(), Is.EqualTo("2.3.4"));
    }

    [Test,AutoMoqData]
    public void EqualsShouldReturnTrueForTwoEqualVersions()
    {
        var one = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        var two = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        Assert.That(one.Equals(two), Is.True);
    }

    [Test,AutoMoqData]
    public void EqualsShouldReturnFalseForTwoDifferentVersions()
    {
        var one = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        var two = new SemanticBrowserVersion(SemVersion.Parse("2.3.5", SemVersionStyles.Strict));
        Assert.That(one.Equals(two), Is.False);
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnANegativeForAHigherVersion()
    {
        var one = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        var two = new SemanticBrowserVersion(SemVersion.Parse("2.3.5", SemVersionStyles.Strict));
        Assert.That(one.CompareTo(two), Is.LessThan(0));
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnAPositiveForALowerVersion()
    {
        var one = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        var two = new SemanticBrowserVersion(SemVersion.Parse("2.3.3", SemVersionStyles.Strict));
        Assert.That(one.CompareTo(two), Is.GreaterThan(0));
    }

    [Test,AutoMoqData]
    public void CompareToShouldReturnZeroForAnEqualVersion()
    {
        var one = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        var two = new SemanticBrowserVersion(SemVersion.Parse("2.3.4", SemVersionStyles.Strict));
        Assert.That(one.CompareTo(two), Is.Zero);
    }

    [Test,AutoMoqData]
    public void TryParseShouldReturnTrueForAValidVersion()
    {
        Assert.That(SemanticBrowserVersion.TryParse("v3.4.5-prerelease+metadata", out _), Is.True);
    }

    [Test,AutoMoqData]
    public void TryParseShouldReturnFalseForAnInvalidVersion()
    {
        Assert.That(SemanticBrowserVersion.TryParse("Elephants", out _), Is.False);
    }
}