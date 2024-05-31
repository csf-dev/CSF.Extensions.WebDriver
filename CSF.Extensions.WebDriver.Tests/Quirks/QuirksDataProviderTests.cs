namespace CSF.Extensions.WebDriver.Quirks;

[TestFixture,Parallelizable]
public class QuirksDataProviderTests
{
    [Test,AutoMoqData]
    public void GetQuirksDataShouldReturnCopyOfOptionsDataIfOnlyOptionsProvided()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldReturnCopyOfStaticDataIfOnlyDataProvided()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldBeAbleToMergeTwoQuirksTogether()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldAllowOptionsDataToShadowStaticData()
    {
        Assert.Fail("Write this test");
    }
}