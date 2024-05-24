using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Configuration;

namespace CSF.Extensions.WebDriver
{
    public class WebDriverFactory
    {
        public WebDriverFactory()
        {
            var driver = new ChromeDriver(new ChromeOptions { });
        }
    }
}

