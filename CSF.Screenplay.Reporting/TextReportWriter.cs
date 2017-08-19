﻿using System;
using System.IO;
using System.Linq;
using CSF.Screenplay.Reporting.Models;

namespace CSF.Screenplay.Reporting
{
  /// <summary>
  /// An <see cref="IReportWriter"/> which writes the report to a <c>System.IO.TextWriter</c>, be that a text file or
  /// an output stream.  This writer uses a simple text-based format to render the results of the report in a
  /// human-readable format.
  /// </summary>
  public class TextReportWriter : IReportWriter
  {
    const char INDENT_CHAR = ' ';
    const int INDENT_WIDTH = 4, RESULT_OR_FAILURE_INDENT_WIDTH = 6;

    readonly TextWriter writer;

    /// <summary>
    /// Write the specified report to the text writer.
    /// </summary>
    /// <param name="reportModel">Report model.</param>
    public void Write(Report reportModel)
    {
      foreach(var scenario in reportModel.Scenarios)
      {
        WriteScenario(scenario);
      }
    }

    void WriteScenario(Models.Scenario scenario)
    {
      if(scenario == null)
        throw new ArgumentNullException(nameof(scenario));

      WriteScenarioHeader(scenario);

      foreach(var reportable in scenario.Reportables)
      {
        WriteReportable(reportable);
      }
    }

    void WriteScenarioHeader(Models.Scenario scenario)
    {
      writer.WriteLine();

      var featureText = GetFeatureText(scenario);
      if(featureText != null)
        writer.WriteLine(featureText);

      writer.WriteLine(GetScenarioText(scenario));

      WriteScenarioOutcome(scenario);
    }

    string GetFeatureText(Models.Scenario scenario)
    {
      if(scenario.FeatureName != null)
        return $"Feature:  {scenario.FeatureName}";

      return null;
    }

    string GetScenarioText(Models.Scenario scenario)
    {
      return $"Scenario: {scenario.FriendlyName?? scenario.Id}";
    }

    void WriteReportable(Reportable reportable, int currentIndentLevel = 0)
    {
      if(reportable == null)
        throw new ArgumentNullException(nameof(reportable));

      if(reportable is GainAbility)
        WriteGainAbility((GainAbility) reportable, currentIndentLevel);
      else if(reportable is Performance)
        WritePerformance((Performance) reportable, currentIndentLevel);
      else
        throw new ArgumentException($"The reportable must be either a {typeof(GainAbility).Name} or a {typeof(Performance).Name}.");
    }

    void WriteGainAbility(GainAbility reportable, int currentIndentLevel)
    {
      WriteIndent(currentIndentLevel);
      WritePerformanceType(reportable, currentIndentLevel);

      writer.Write(reportable.Ability.GetReport(reportable.Actor));
      writer.WriteLine();
    }

    void WritePerformance(Performance reportable, int currentIndentLevel)
    {
      WriteIndent(currentIndentLevel);
      WritePerformanceType(reportable, currentIndentLevel);

      writer.Write(reportable.Performable.GetReport(reportable.Actor));
      writer.WriteLine();

      if(reportable.Outcome == Outcome.SuccessWithResult)
        WriteResult(reportable, currentIndentLevel);
      else if(reportable.Outcome == Outcome.Failure || reportable.Outcome == Outcome.FailureWithException)
        WriteFailure(reportable, currentIndentLevel);

      foreach(var child in reportable.Reportables)
      {
        WriteReportable(child, currentIndentLevel + 1);
      }
    }

    string GetPerformanceTypeString(PerformanceType type, int currentIndentLevel)
    {
      if(type == PerformanceType.Unspecified)
        return String.Empty;

      // Don't keep writing the performance type after the base indent level
      if(currentIndentLevel > 0)
        return String.Empty;

      return type.ToString();
    }

    void WriteScenarioOutcome(Models.Scenario scenario)
    {
      writer.WriteLine("**** {0} ****", GetScenarioOutcome(scenario));
    }

    string GetScenarioOutcome(Models.Scenario scenario) => scenario.IsSuccess? "Success" : "Failure";

    void WriteIndent(int currentLevel)
    {
      writer.Write(GetIndent(currentLevel));
    }

    void WriteResultOrFailureIndent(int currentLevel)
    {
      WriteIndent(currentLevel);
      writer.Write(new String(INDENT_CHAR, RESULT_OR_FAILURE_INDENT_WIDTH));
    }

    void WritePerformanceType(Reportable reportable, int currentIndentLevel)
    {
      writer.Write("{0,5} ", GetPerformanceTypeString(reportable.PerformanceType, currentIndentLevel));
    }

    void WriteResult(Performance reportable, int currentIndentLevel)
    {
      WriteResultOrFailureIndent(currentIndentLevel);
      var result = reportable.Result?.ToString();
      writer.WriteLine("Result:{0}", result ?? "<null>");
    }

    void WriteFailure(Performance reportable, int currentIndentLevel)
    {
      WriteResultOrFailureIndent(currentIndentLevel);

      var reportableException = reportable.Exception as IReportable;
      if(reportableException != null)
      {
        writer.WriteLine("FAILED {0}", reportableException.GetReport(reportable.Actor));
        return;
      }

      var exception = reportable.Exception?.ToString();
      if(exception != null)
        writer.WriteLine("FAILED with an exception:\n{0}", exception);
      else
        writer.WriteLine("FAILED");
    }

    string GetIndent(int currentLevel) 
      => String.Join(String.Empty, Enumerable.Range(0, currentLevel).Select(x => new String(INDENT_CHAR, INDENT_WIDTH)));

    /// <summary>
    /// Initializes a new instance of the <see cref="TextReportWriter"/> class.
    /// </summary>
    /// <param name="writer">Writer.</param>
    public TextReportWriter(TextWriter writer)
    {
      if(writer == null)
        throw new ArgumentNullException(nameof(writer));

      this.writer = writer;
    }
  }
}
