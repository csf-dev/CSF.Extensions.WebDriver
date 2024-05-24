using System;
using System.Collections.Generic;
using System.Linq;
using CSF.Extensions.WebDriver.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Remote;

namespace OpenQA.Selenium
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services)
        {
            services.AddTransient<IGetsWebDriverWithDeterministicOptionsTypes, SeleniumDriverAndOptionsScanner>();
            services.ConfigureOptions<WebDriverCreationConfigureOptions>();
            services.AddOptions<WebDriverCreationOptionsCollection>();

            throw new NotImplementedException();
        }

        // I'd like to add two more overloads of the above, to deal with custom configuration paths and perhaps some post-configuration of the options

        // public static IServiceCollection AddWebDriverFactory(this IServiceCollection services,
        //                                                      IConfiguration configurationSection,
        //                                                      Action<WebDriverCreationOptionsCollection> configureOptions = null)
        // {
        //     services.AddOptions<WebDriverCreationOptionsCollection>()
        //         .Configure(opts => {
        //             BindOptions(opts, configurationSection);
        //             configureOptions?.Invoke(opts);
        //         });
        
        // }

        
        //  string configPath = "WebDriverFactory",
        //  Action<WebDriverCreationOptionsCollection> configureOptions = null
    }


}

