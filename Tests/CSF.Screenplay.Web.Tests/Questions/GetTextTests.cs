﻿using System;
using CSF.Screenplay.Web.Builders;
using CSF.Screenplay.Web.Tests.Pages;
using NUnit.Framework;
using FluentAssertions;
using static CSF.Screenplay.StepComposer;

namespace CSF.Screenplay.Web.Tests.Questions
{
  [TestFixture]
  public class GetTextTests
  {
    Actor joe;

    [SetUp]
    public void Setup()
    {
      joe = WebdriverTestSetup.GetJoe();
    }

    [Test,Reportable]
    public void GetText_returns_expected_value()
    {
      Given(joe).WasAbleTo(OpenTheirBrowserOn.ThePage<HomePage>());

      Then(joe).ShouldSee(TheText.Of(HomePage.ImportantString)).Should().Be("banana!");
    }

    [Test,Reportable]
    public void GetConvertedText_returns_expected_value()
    {
      Given(joe).WasAbleTo(OpenTheirBrowserOn.ThePage<HomePage>());

      Then(joe).ShouldSee(TheText.From(HomePage.ImportantNumber).As<int>()).Should().Be(42);
    }
  }
}
