# Web driver extras
Web driver extras is a set of support and utility types for **Selenium Web Driver**, a suite for browser automation, particularly useful for the testing of web applications.

This library is particularly aimed at providing an abstract way of [creating and configuring] `IWebDriver` instances, in such a way that test logic does not need to know anything about which web browser is in use, or how it has been configured.

It also provides [an attempted solution] to deal with some of the quirks and differences in behaviour between web browsers.

Finally it offers an intgeration with a popular cloud-based provider of web drivers (free for open source projects): [Sauce Labs].

[creating and configuring]: https://github.com/csf-dev/WebDriverExtras/wiki/WebDriverFactories
[an attempted solution]: https://github.com/csf-dev/WebDriverExtras/wiki/WebBrowserFlags
[Sauce Labs]: https://github.com/csf-dev/WebDriverExtras/wiki/SauceLabsIntegration

## History
Web driver extras was originally part of another project - for the **[Screenplay testing pattern]** - but it has since been forked away into a project of its own.  Screenplay still makes use of this library as a NuGet package.

[Screenplay testing pattern]: https://github.com/csf-dev/CSF.Screenplay