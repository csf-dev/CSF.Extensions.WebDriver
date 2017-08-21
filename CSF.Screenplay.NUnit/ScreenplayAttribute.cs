﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CSF.Screenplay.Integration;
using CSF.Screenplay.Scenarios;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace CSF.Screenplay.NUnit
{
  /// <summary>
  /// Applied to an assembly, fixture or test - this indicates that a test (or all of the tests within the
  /// scope of this attribute) are Screenplay tests.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class ScreenplayAttribute : Attribute, ITestAction, ITestBuilder
  {
    const string ScreenplayScenarioKey = "Current scenario";
    IScreenplayIntegration cachedIntegration;

    /// <summary>
    /// Gets the targets for the attribute (when performing before/after test actions).
    /// </summary>
    /// <value>The targets.</value>
    public ActionTargets Targets => ActionTargets.Test;

    /// <summary>
    /// Performs actions before each test.
    /// </summary>
    /// <param name="test">Test.</param>
    public void BeforeTest(ITest test)
    {
      var scenario = GetScenario(test);
      GetIntegration(test).BeforeScenario(scenario);
    }

    /// <summary>
    /// Performs actions after each test.
    /// </summary>
    /// <param name="test">Test.</param>
    public void AfterTest(ITest test)
    {
      var scenario = GetScenario(test);
      var success = GetScenarioSuccess(test);
      GetIntegration(test).AfterScenario(scenario, success);
    }

    /// <summary>
    /// Builds a collectio of NUnit test methods from metadata about the test.
    /// </summary>
    /// <returns>The test methods.</returns>
    /// <param name="method">Method information.</param>
    /// <param name="suite">Test suite information.</param>
    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
    {
      GetIntegration(method).EnsureServicesAreRegistered();

      var scenario = CreateScenario(method, suite);

      suite.Properties.Add(ScreenplayScenarioKey, scenario);

      return BuildFrom(method, suite, scenario);
    }

    IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite, ScreenplayScenario scenario)
    {
      var builder = new NUnitTestCaseBuilder();
      var tcParams = new TestCaseParameters(new [] { scenario });

      var testMethod = builder.BuildTestMethod(method, suite, tcParams);

      return new [] { testMethod };
    }

    bool GetScenarioSuccess(ITest test)
    {
      var result = TestContext.CurrentContext.Result;
      return result.Outcome.Status == TestStatus.Passed;
    }

    ScreenplayScenario GetScenario(ITest test)
    {
      if(!test.Properties.ContainsKey(ScreenplayScenarioKey))
      {
        var message = $"The test instance must contain an instance of `{nameof(ScreenplayScenario)}' in " +
                      $"its {nameof(ITest.Properties)}.";
        throw new ArgumentException(message, nameof(test));
      }

      return (ScreenplayScenario) test.Properties.Get(ScreenplayScenarioKey);
    }

    ScreenplayScenario CreateScenario(IMethodInfo method, Test suite)
    {
      var testAdapter = new SuitAndMethodScenarioAdapter(suite, method);
      var featureName = GetFeatureName(testAdapter);
      var scenarioName = GetScenarioName(testAdapter);
      var factory = GetIntegration(method).GetScenarioFactory();

      return factory.GetScenario(featureName, scenarioName);
    }

    IScreenplayIntegration GetIntegration(IMethodInfo method)
    {
      if(cachedIntegration == null)
      {
        var assembly = method?.MethodInfo?.DeclaringType?.Assembly;
        if(assembly == null)
        {
          throw new ArgumentException($"The method must have an associated {nameof(Assembly)}.",
                                      nameof(method));
        }

        var assemblyAttrib = assembly.GetCustomAttribute<ScreenplayAssemblyAttribute>();
        if(assemblyAttrib == null)
        {
          var message = $"All test methods must be contained within assemblies which are " +
                        $"decorated with `{nameof(ScreenplayAssemblyAttribute)}'.";
          throw new InvalidOperationException(message);
        }
        
        cachedIntegration = assemblyAttrib.Integration;
      }

      return cachedIntegration;
    }

    IScreenplayIntegration GetIntegration(ITest test)
    {
      if(test.Method == null)
        throw new ArgumentException("The test must specify a method.", nameof(test));

      return GetIntegration(test.Method);
    }

    IdAndName GetFeatureName(IScenarioAdapter test) => new IdAndName(test.FeatureId, test.FeatureName);

    IdAndName GetScenarioName(IScenarioAdapter test) => new IdAndName(test.ScenarioId, test.ScenarioName);
  }
}
