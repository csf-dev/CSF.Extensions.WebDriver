# WebDriver identification

Most implementations of `IWebDriver` also implement `IHasCapabilities` and have capabilities indicating the browser name, version and platform.
These values are strings though, which is particularly troublesome for the browser version when we need to answer questions such as _is this browser between versions X and Y?_

When using [the universal WebDriver factory], the returned WebDriver is enhanced with an additional interface: [`IHasBrowserId`].
This interface provides a get-only property of type [`BrowserId`], which in-turn provides a [`BrowserVersion`].
This enhancement may be disabled by setting [`AddBrowserIdentification`] in the WebDriver creation options to `false`.

[the universal WebDriver factory]: index.md
[`IHasBrowserId`]: xref:CSF.Extensions.WebDriver.Identification.IHasBrowserId
[`BrowserId`]: xref:CSF.Extensions.WebDriver.Identification.BrowserId
[`BrowserVersion`]: xref:CSF.Extensions.WebDriver.Identification.BrowserVersion
[`AddBrowserIdentification`]: xref:CSF.Extensions.WebDriver.Factories.WebDriverCreationOptions.AddBrowserIdentification

## A note on proxies

Be aware that when the WebDriver identification functionality is activated, [the WebDriver returned by the universal factory will be _a proxy object_] and not the original concrete WebDriver implementation.
In best-practice scenarios where the WebDriver is utilised only by its interfaces this should make no difference.
More information is available at the linked documentation above.

[the WebDriver returned by the universal factory will be _a proxy object_]: Proxies.md
