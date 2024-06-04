# Universal WebDriver factory

A common requirement when performing browser testing is to conduct tests using a variety of browsers.
This helps ensure that app functionality is not reliant upon a particular browser feature or quirk and it's truly cross-browser compatible. 

The universal WebDriver factory is a configuration-driven mechanism by which WebDriver instances may be constructed. 
It is based upon Microsoft [Dependency Injection], the [Options Pattern] and [Configuration].

[Dependency Injection]: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
[Options Pattern]: https://learn.microsoft.com/en-us/dotnet/core/extensions/options
[Configuration]: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration

## Usage

### Add the factory to dependency injection

Use either [`AddWebDriverFactory`] or [`AddWebDriverFactoryWithoutOptionsPattern`] to set the universal factory up in your service collection.

```csharp
services.AddWebDriverFactory();
```

There are overloads of `AddWebDriverFactory` available to:

* Specify a non-default configuration path for the WebDriver factory options; the default is `WebDriverFactory`
* Specify a configuration section from which to build the WebDriver factory options
* Specify an additional configuration callback to provide extra options outside the configuration system


[`AddWebDriverFactory`]: xref:CSF.Extensions.WebDriver.ServiceCollectionExtensions.AddWebDriverFactory(IServiceCollection,string,Action<WebDriverCreationOptionsCollection>)
[`AddWebDriverFactoryWithoutOptionsPattern`]: xref:CSF.Extensions.WebDriver.ServiceCollectionExtensions.AddWebDriverFactoryWithoutOptionsPattern()

## Include configuration for one or more WebDrivers

This configuration should be written using whichever configuration mechanism you wish to use. 
Here is an example using the common `appsettings.json` format:

```json

```

You may set one of your configurations to be 'the selected default' if you wish, enabling you to use [`GetDefaultWebDriver()`].
Do not forget that you may provide configuration from multiple sources; for example you may specify your available driver configurations in a JSON file but specify the default selected one via a command-line parameter such as `--WebDriverFactory::SelectedConfiguration MyConfiguration`.