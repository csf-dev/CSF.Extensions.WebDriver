using AutoFixture;
using AutoFixture.AutoMoq;

namespace CSF.Extensions.WebDriver;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization())) { }
}