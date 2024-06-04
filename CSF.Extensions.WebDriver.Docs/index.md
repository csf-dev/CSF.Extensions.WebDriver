---
_layout: landing
---

# CSF.Extensions.WebDriver

This project offers some support functionality for **[Selenium WebDriver]**.
This is broadly organised into three features:

* A [universal WebDriver factory] intended to help keep WebDriver configuration and construction out of your application/tests
* A mechanism to [reliably identify WebDriver instances] and their version information, after they are created
* A mechanism to ['mark WebDrivers up' with information about their quirks] which affect that browser/WebDriver/version combination

[Selenium WebDriver]: https://www.selenium.dev/documentation/webdriver/
[universal WebDriver factory]: Docs/index.md
[reliably identify WebDriver instances]: Docs/DriverIdentification.md
['mark WebDrivers up' with information about their quirks]: Docs/Quirks.md
