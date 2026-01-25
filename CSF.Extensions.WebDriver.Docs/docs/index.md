# Universal WebDriver factory

A common requirement when performing browser testing is to conduct tests using a variety of browsers.
This helps ensure that app functionality is not reliant upon a particular browser feature or quirk and it's truly cross-browser compatible.
The universal WebDriver factory is a configuration-driven mechanism by which WebDriver instances may be constructed.
It is based upon Microsoft [Dependency Injection] and optionally the [Options Pattern] and [Configuration].

The WebDriver factory is the mechanism by which other functionality in this library is activated.
To begin using it, follow the three steps below.

[Dependency Injection]: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
[Options Pattern]: https://learn.microsoft.com/en-us/dotnet/core/extensions/options
[Configuration]: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration

## 1. Add the factory to dependency injection

It is recommended to use [`AddWebDriverFactory`] with your dependency injection configuration.
This enables the Microsoft Options Pattern and Configuration for the WebDriver factory.
You may alternatively use [`AddWebDriverFactoryWithoutOptionsPattern`] if you do not with to use those technologies, although some features will be unavailable to you if you choose this.

```csharp
services.AddConfiguration();
services.AddWebDriverFactory();
```

There are overloads of `AddWebDriverFactory` available to:

* Specify a non-default configuration path for the WebDriver factory options; the default is `WebDriverFactory`
* Specify a configuration section from which to build the WebDriver factory options
* Specify an additional configuration callback to provide extra options outside the configuration system

Read the documentation for these functions (linked above) for more info.

[`AddWebDriverFactory`]: xref:CSF.Extensions.WebDriver.ServiceCollectionExtensions.AddWebDriverFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.String,System.Action{CSF.Extensions.WebDriver.Factories.WebDriverCreationOptionsCollection})
[`AddWebDriverFactoryWithoutOptionsPattern`]: xref:CSF.Extensions.WebDriver.ServiceCollectionExtensions.AddWebDriverFactoryWithoutOptionsPattern(Microsoft.Extensions.DependencyInjection.IServiceCollection)

## 2. Include configuration for one or more WebDrivers

This configuration should be written using whichever configuration mechanism you wish to use.
Here is an example using the common `appsettings.json` format:

```json
{
    "WebDriverFactory": {
        "DriverConfigurations": {
            "MyRemoteSafari": {
                "DriverType": "RemoteWebDriver",
                "OptionsType": "SafariOptions",
                "GridUrl": "https://gridurl.example.com/url-path"
            },
            "MyLocalChrome": {
                "DriverType": "ChromeDriver"
            }
        },
        "SelectedConfiguration": "MyLocalChrome"
    }
}
```

You may set one of your configurations to be 'the selected default' if you wish, enabling you to use [`GetDefaultWebDriver()`].
Do not forget that you may provide configuration from multiple sources; for example you may specify your available driver configurations in a JSON file but specify the default selected one via a command-line parameter such as:

```txt
--WebDriverFactory::SelectedConfiguration MyConfigurationName
```

> [!TIP]
> Do not store secrets such as passwords in your configuration.
> The methods of [`IGetsWebDriver`] and [`ICreatesWebDriverFromOptions`] provide parameters whereby secrets may be injected into the `DriverOptions` from external sources, such as environment variables.
> This avoids the need to add secrets to source-controlled files.

[`GetDefaultWebDriver()`]: xref:CSF.Extensions.WebDriver.IGetsWebDriver.GetDefaultWebDriver(System.Action{OpenQA.Selenium.DriverOptions})

### Configuration reference

The available configuration options/syntax is documented in the classes [`WebDriverCreationOptionsCollection`] and [`WebDriverCreationOptions`].

[`WebDriverCreationOptionsCollection`]: xref:CSF.Extensions.WebDriver.Factories.WebDriverCreationOptionsCollection
[`WebDriverCreationOptions`]: xref:CSF.Extensions.WebDriver.Factories.WebDriverCreationOptions

## 3. Inject and use the services

Use dependency injection to inject an [`IGetsWebDriver`].
Use this service to get WebDriver instances.

`IGetsWebDriver` is unavailable if you used [`AddWebDriverFactoryWithoutOptionsPattern`] when setting this functionality up.
In that case you must use [`ICreatesWebDriverFromOptions`] instead.
This service offers the same functionality except that the consumer is responsible for specifying the [`WebDriverCreationOptions`]; they are not retrieved from Options.

[`IGetsWebDriver`]: xref:CSF.Extensions.WebDriver.IGetsWebDriver
[`ICreatesWebDriverFromOptions`]: xref:CSF.Extensions.WebDriver.Factories.ICreatesWebDriverFromOptions
