---
uid: WebBrowserQuirksArticle
---

# WebDriver quirks

_There are a lot of web browsers and browser versions out there!_
Unfortunately they do not all behave in a perfectly uniform manner; some WebDriver implementations have bugs and some just have oddities which are unique to them.
A real (albeit now-outdated) example is that [WebDriver for Apple Safari v11 could not change the selection of an HTML `<select>` element].
When faced with that bug, the only course of action a developer could take was to work around it.

[WebDriver for Apple Safari v11 could not change the selection of an HTML `<select>` element]: https://github.com/SeleniumHQ/selenium/issues/5475#issuecomment-365082942

## How the 'quirks' architecture can help

Developers do not want to litter their WebDriver-consuming code with browser detection logic.
Just like in regular web development, [browser detection is bad, feature detection is better].
What the quirks architecture provides is an additional interface, added to WebDrivers created by [the universal WebDriver factory]: [`IHasQuirks`].

WebDrivers which implement `IHasQuirks` can cross-reference their [browser identification] with source data listing which browsers are affected by which quirks.
The result is the [`AllQuirks`] property and the following extension methods (for convenience):

* [`HasQuirk(string)`]
* [`GetQuirks()`]
* [`GetFirstApplicableQuirk(params string［］)`]

[browser detection is bad, feature detection is better]: https://developer.mozilla.org/en-US/docs/Learn/Tools_and_testing/Cross_browser_testing/Feature_detection
[the universal WebDriver factory]: index.md
[`IHasQuirks`]: xref:CSF.Extensions.WebDriver.Quirks.IHasQuirks
[browser identification]: DriverIdentification.md
[`AllQuirks`]: xref:CSF.Extensions.WebDriver.Quirks.IHasQuirks.AllQuirks

[`HasQuirk(string)`]: xref:OpenQA.Selenium.WebDriverExtensions.HasQuirk(OpenQA.Selenium.IWebDriver,System.String)
[`GetQuirks()`]: xref:OpenQA.Selenium.WebDriverExtensions.GetQuirks(OpenQA.Selenium.IWebDriver)
[`GetFirstApplicableQuirk(params string［］)`]: xref:OpenQA.Selenium.WebDriverExtensions.GetFirstApplicableQuirk(OpenQA.Selenium.IWebDriver,System.String[])

## The quirks source data

Quirks source data may come from two sources.
To use quirks _at least one source must be activated_ and it is recommended to enable both.

* Hard-coded into an application/library
* Supplementary configuration data

The intent is that an application or library may ship with quirks information that is known at the time of writing.
This information may be supplemented or (in part or wholly) overwritten by quirks information provided by the consumer.
This allows consuming logic to react to changes in browser quirks (as time moves on) by adding their own quirks configuration and not needing to wait for an upstream app/library to release an updated version with new quirks source data.

This library uses a simple merging algorithm to combine the hard-coded and options data-sources.
Where the two sources list quirks that the other source does not, the resultant data will contain both quirks.
Where the two sources list the same quirk, the Options data will _win the disagreement_, so to speak, and will shadow the hard-coded data.

Developers may use this technique to update the affected browsers for a quirk or even to (effectively) remove it, by giving it an empty set of affected browsers.

## Setting up quirks functionality

To configure the source data you must activate it in source control.
In the following example, `quirksData` represents data which would be hard-coded into your application/library (design-time).

```csharp
services.AddWebDriverQuirks(quirksData);
```

The [`AddWebDriverQuirks`] method is customisable with a number of parameters, most of which are not shown above.
By default any quirks data specified in [the app Configuration], via [the Options Pattern], will be used to supplement and/or override that hard-coded data.
The default configuration path for quirks data is `WebDriverQuirks`.

Lastly, to use quirks functionality it must also be activated in the [`WebDriverCreationOptions`], via the [`AddBrowserQuirks`] property.

[`AddWebDriverQuirks`]: xref:CSF.Extensions.WebDriver.ServiceCollectionExtensions.AddWebDriverQuirks(Microsoft.Extensions.DependencyInjection.IServiceCollection,CSF.Extensions.WebDriver.Quirks.QuirksData,System.Boolean,System.String)
[the app Configuration]: https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration
[the Options Pattern]: https://learn.microsoft.com/en-us/dotnet/core/extensions/options
[`WebDriverCreationOptions`]: xref:CSF.Extensions.WebDriver.Factories.WebDriverCreationOptions
[`AddBrowserQuirks`]: xref:CSF.Extensions.WebDriver.Factories.WebDriverCreationOptions.AddBrowserQuirks

## A note on proxies

Be aware that when the WebDriver quirks functionality is activated, [the WebDriver returned by the universal factory will be _a proxy object_] and not the original concrete WebDriver implementation.
In best-practice scenarios where the WebDriver is utilised only by its interfaces this should make no difference.
More information is available at the linked documentation above.

[the WebDriver returned by the universal factory will be _a proxy object_]: Proxies.md
