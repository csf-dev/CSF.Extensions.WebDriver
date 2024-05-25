using System;
using CSF.Extensions.WebDriver.Factories;
using Microsoft.Extensions.Options;

namespace CSF.Extensions.WebDriver
{
    public class WebDriverFactory
    {
        readonly IOptions<WebDriverCreationConfigureOptions> options;

        public WebDriverFactory(IOptions<WebDriverCreationConfigureOptions> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }
    }
}

