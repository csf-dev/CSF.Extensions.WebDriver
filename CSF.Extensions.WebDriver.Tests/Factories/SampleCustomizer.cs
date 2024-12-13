using OpenQA.Selenium.Chrome;

namespace CSF.Extensions.WebDriver.Factories;

public class SampleCustomizer : ICustomizesOptions<ChromeOptions>
{
    public void CustomizeOptions(ChromeOptions options) { /* Intentional no-op */ }
}