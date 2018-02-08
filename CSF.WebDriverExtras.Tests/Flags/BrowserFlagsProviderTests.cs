﻿using System;
using System.Collections.Generic;
using CSF.WebDriverExtras.BrowserId;
using CSF.WebDriverExtras.Flags;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace CSF.WebDriverExtras.Tests.Flags
{
  [TestFixture,Parallelizable(ParallelScope.All)]
  public class BrowserFlagsProviderTests
  {
    [Test,AutoMoqData]
    public void GetFlags_returns_flags_added_by_single_provider(BrowserIdentification browserId,
                                                                IHasCapabilities webDriver,
                                                                IGetsBrowserIdentification idFactory,
                                                                List<string> flags)
    {
      // Arrange
      SetupToReturnBrowserId(idFactory, webDriver, browserId);

      var definition = new FlagsDefinition();
      definition.BrowserNames.Add(browserId.Name);
      definition.AddFlags.UnionWith(flags);

      var sut = new BrowserFlagsProvider(new [] { definition }, idFactory);

      // Act
      var result = sut.GetFlags(webDriver);

      // Assert
      Assert.That(result, Is.EquivalentTo(flags));
    }

    [Test,AutoMoqData]
    public void GetFlags_returns_flags_added_by_two_providers(BrowserIdentification browserId,
                                                                IHasCapabilities webDriver,
                                                                IGetsBrowserIdentification idFactory)
    {
      // Arrange
      SetupToReturnBrowserId(idFactory, webDriver, browserId);

      var definitionOne = new FlagsDefinition();
      definitionOne.BrowserNames.Add(browserId.Name);
      definitionOne.AddFlags.Add("One");

      var definitionTwo = new FlagsDefinition();
      definitionTwo.BrowserNames.Add(browserId.Name);
      definitionTwo.AddFlags.Add("Two");

      var sut = new BrowserFlagsProvider(new [] { definitionOne, definitionTwo }, idFactory);

      var expected = new [] { "One", "Two" };

      // Act
      var result = sut.GetFlags(webDriver);

      // Assert
      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test,AutoMoqData]
    public void GetFlags_ignores_flags_added_by_non_matching_provider(BrowserIdentification browserId,
                                                                      IHasCapabilities webDriver,
                                                                      IGetsBrowserIdentification idFactory,
                                                                      string otherBrowserName)
    {
      // Arrange
      SetupToReturnBrowserId(idFactory, webDriver, browserId);

      var definitionOne = new FlagsDefinition();
      definitionOne.BrowserNames.Add(browserId.Name);
      definitionOne.AddFlags.Add("One");

      var definitionTwo = new FlagsDefinition();
      definitionTwo.BrowserNames.Add(otherBrowserName);
      definitionTwo.AddFlags.Add("Two");

      var sut = new BrowserFlagsProvider(new [] { definitionOne, definitionTwo }, idFactory);

      var expected = new [] { "One" };

      // Act
      var result = sut.GetFlags(webDriver);

      // Assert
      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test,AutoMoqData]
    public void GetFlags_does_not_include_flags_removed_by_same_provider(BrowserIdentification browserId,
                                                                         IHasCapabilities webDriver,
                                                                         IGetsBrowserIdentification idFactory)
    {
      // Arrange
      SetupToReturnBrowserId(idFactory, webDriver, browserId);

      var definitionOne = new FlagsDefinition();
      definitionOne.BrowserNames.Add(browserId.Name);
      definitionOne.AddFlags.Add("One");
      definitionOne.RemoveFlags.Add("One");

      var definitionTwo = new FlagsDefinition();
      definitionTwo.BrowserNames.Add(browserId.Name);
      definitionTwo.AddFlags.Add("Two");

      var sut = new BrowserFlagsProvider(new [] { definitionOne, definitionTwo }, idFactory);

      var expected = new [] { "Two" };

      // Act
      var result = sut.GetFlags(webDriver);

      // Assert
      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test,AutoMoqData]
    public void GetFlags_does_not_include_flags_removed_by_other_provider(BrowserIdentification browserId,
                                                                          IHasCapabilities webDriver,
                                                                          IGetsBrowserIdentification idFactory)
    {
      // Arrange
      SetupToReturnBrowserId(idFactory, webDriver, browserId);

      var definitionOne = new FlagsDefinition();
      definitionOne.BrowserNames.Add(browserId.Name);
      definitionOne.AddFlags.Add("One");

      var definitionTwo = new FlagsDefinition();
      definitionTwo.BrowserNames.Add(browserId.Name);
      definitionTwo.AddFlags.Add("Two");
      definitionTwo.RemoveFlags.Add("One");

      var sut = new BrowserFlagsProvider(new [] { definitionOne, definitionTwo }, idFactory);

      var expected = new [] { "Two" };

      // Act
      var result = sut.GetFlags(webDriver);

      // Assert
      Assert.That(result, Is.EquivalentTo(expected));
    }

    void SetupToReturnBrowserId(IGetsBrowserIdentification idFactory,
                                IHasCapabilities webDriver,
                                BrowserIdentification browserId)
    {
      Mock.Get(idFactory)
          .Setup(x => x.GetIdentification(webDriver))
          .Returns(browserId);
    }
  }
}
