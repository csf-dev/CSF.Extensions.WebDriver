﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace CSF.WebDriverExtras.Impl
{
  /// <summary>
  /// Implementation of <see cref="IWebDriverFactory"/> which gets an Internet Explorer web driver.
  /// </summary>
  public class InternetExplorerWebDriverFactory : IWebDriverFactory
  {

    /// <summary>
    /// Gets the name of the web browser that this factory will create.
    /// </summary>
    /// <returns>The browser name.</returns>
    public string GetBrowserName() => "Internet explorer";

    /// <summary>
    /// Gets the web driver.
    /// </summary>
    /// <returns>The web driver.</returns>
    public IWebDriver GetWebDriver()
    {
      return GetWebDriver(null);
    }

    /// <summary>
    /// Gets the web driver.
    /// </summary>
    /// <returns>The web driver.</returns>
    public IWebDriver GetWebDriver(IDictionary<string,object> capabilities)
    {
      var driverService = GetDriverService();
      var options = GetIEOptions();

      if(capabilities != null)
      {
        foreach(var cap in capabilities)
        {
          options.AddAdditionalCapability(cap.Key, cap.Value);
        }
      }

      var timeout = GetTimeout();
      return new InternetExplorerDriver(driverService, options, timeout);
    }

    TimeSpan GetTimeout()
    {
      return TimeSpan.FromSeconds(CommandTimeoutSeconds);
    }

    InternetExplorerDriverService GetDriverService()
    {
      InternetExplorerDriverService output;

      if(String.IsNullOrEmpty(DriverPath))
        output = InternetExplorerDriverService.CreateDefaultService();
      else
        output = InternetExplorerDriverService.CreateDefaultService(DriverPath);

      output.HideCommandPromptWindow = true;
      output.SuppressInitialDiagnosticInformation = true;

      if(DriverPort.HasValue)
        output.Port = DriverPort.Value;

      return output;
    }

    InternetExplorerOptions GetIEOptions()
    {
      var output = new InternetExplorerOptions();

      return output;
    }
  }
}
