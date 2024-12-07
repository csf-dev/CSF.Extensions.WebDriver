# WebDriver extensions

This small library provides some utility functionality for **[Selenium WebDriver]**.
These features may be used individually or together.

* [A universal WebDriver factory]
* [A mechanism for dealing with browser-specific quirks]
* [Types for convenient identification of browsers & versions]

The types in this library integrate with some commonly-used .NET technologies: 

* [Dependency injection]
* [The Options Pattern]
* [.NET Configuration]

[Selenium WebDriver]: https://www.selenium.dev/documentation/webdriver/
[A universal WebDriver factory]: https://csf-dev.github.io/CSF.Extensions.WebDriver/docs/index.html
[A mechanism for dealing with browser-specific quirks]: https://csf-dev.github.io/CSF.Extensions.WebDriver/docs/Quirks.html
[Types for convenient identification of browsers & versions]: https://csf-dev.github.io/CSF.Extensions.WebDriver/docs/DriverIdentification.html
[Dependency injection]: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
[The Options Pattern]: https://learn.microsoft.com/en-us/dotnet/core/extensions/options
[.NET Configuration]: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration

## Usages

This functionality may be especially useful to those who are interested in using Selenium with a wide range of browsers and/or WebDriver implementations. 

* The universal factory allows you to keep your target WebDriver/browser configurations out of code.
* The quirks mechanism allows for fine-grained and tightly-targeted application of workarounds for differences in behaviour or limitations which are specific to a small number of browser/driver/version ranges.
    * The use of configuration data here allows for quick configuration-based override of which browsers/versions are affected by which quirks. This is useful as browser versions are released at high velocity and _things change_.
* The browser identification mechanism is primarily a dependency of the quirks mechanism but may be used standalone.
