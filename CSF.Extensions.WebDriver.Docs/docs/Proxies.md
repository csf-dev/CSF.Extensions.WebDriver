# Proxied WebDrivers

Two of the functions of the universal factory require adding additional interfaces to the WebDriver:

* [Browser identification]
* [WebDriver quirks]

The only sensible way to do this at runtime, without either disrupting other interfaces which were present on the WebDriver or [being forced to violate LSP], is to make use of a proxying library.
In the case of CSF.Extensions.WebDriver, [Castle DynamicProxy] is used.

What this means is that when the [universal WebDriver factory] returns a WebDriver, if either or both of the functionalities above are enabled, then the WebDriver returned will be a proxy object and not the original concrete implementation of `IWebDriver`.

[Browser identification]: DriverIdentification.md
[WebDriver quirks]: Quirks.md
[being forced to violate LSP]: https://en.wikipedia.org/wiki/Liskov_substitution_principle
[Castle DynamicProxy]: https://www.castleproject.org/projects/dynamicproxy/
[universal WebDriver factory]: index.md

## Consequences

Take the following code as an example, the first two lines would create a minimal default local Chrome WebDriver.

```csharp
// Imagine this factory has been dependency-injected
ICreatesWebDriverFromOptions factory;

var webDriver = factory.GetWebDriver(new () { DriverType = "ChromeDriver" });
var hasActiveDevTools = ((ChromeDriver) webDriver).HasActiveDevToolsSession;
```

_The last line of code above would crash_ with an `InvalidCastException`.
That's because the `webDriver` object is not an instance of `ChromeDriver`, it is a proxy object wrapping that `ChromeDriver` instance.

When using Selenium, and in software development in general, it is bad practice to depend upon concrete classes when interfaces are available.
In well-written logic which depends upon only interfaces, the limitation above does not come into play.
_Proxy WebDrivers have all of the same interfaces as the WebDriver with which they were created_ and provide the same functionality for all of them.
Those interfaces are detected upon the WebDriver as the proxy is created, so third party WebDrivers with additional/unknown interfaces would also be supported.

## Unproxying

If you encounter an (unexpected) situation where the proxied WebDriver causes a problem, this library provides a mechanism of getting the original 'unproxied' WebDriver:

```csharp
var unproxied = maybeProxy.Unproxy();
```

In the example above, if `maybeProxy` was a proxied WebDriver, `unproxied` is now the original WebDriver instance which was wrapped by the proxy.
The [`Unproxy()`] extension method is safe to use on both proxy and non-proxy WebDrivers.
If used upon a WebDriver which is not a proxy then it simply does nothing and returns the same WebDriver instance.

[`Unproxy()`]: xref:OpenQA.Selenium.WebDriverExtensions.Unproxy(OpenQA.Selenium.IWebDriver)
