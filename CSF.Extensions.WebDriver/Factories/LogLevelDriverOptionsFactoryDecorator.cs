using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Decorator for <see cref="ICreatesDriverOptions"/> which conditionally sets the logging preference for the
    /// options, based upon <see cref="WebDriverCreationOptions.BrowserLogLevel"/>.
    /// </summary>
    public class LogLevelDriverOptionsFactoryDecorator : ICreatesDriverOptions
    {
        readonly ICreatesDriverOptions wrapped;

        /// <inheritdoc/>
        public DriverOptions CreateOptions(Type optionsType, IConfigurationSection config)
        {
            var options = wrapped.CreateOptions(optionsType, config);
            var logLevel = config.GetValue<string>(nameof(WebDriverCreationOptions.BrowserLogLevel));
            if(logLevel != null && Enum.TryParse<LogLevel>(logLevel, out var parsedLevel))
                options.SetLoggingPreference(LogType.Browser, parsedLevel);
            return options;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="LogLevelDriverOptionsFactoryDecorator"/>.
        /// </summary>
        /// <param name="wrapped">The wrapped service</param>
        /// <exception cref="ArgumentNullException">If <paramref name="wrapped"/> is <see langword="null"/>.</exception>
        public LogLevelDriverOptionsFactoryDecorator(ICreatesDriverOptions wrapped)
        {
            this.wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
        }
    }
}