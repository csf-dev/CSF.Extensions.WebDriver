using System.Reflection;
using AutoFixture;
using Microsoft.Extensions.Logging;

namespace CSF.Extensions.WebDriver;

/// <summary>
/// Customizes an <see cref="ILogger{TCategoryName}"/> such that it will log to the console in a manner suitable for NUnit.
/// </summary>
/// <remarks>
/// <para>
/// Beware using <c>Console.WriteLine</c> or the MS Extensions Logging Console Logger.  The console logger performs some thread-management which
/// is incompatible with NUnit: https://github.com/nunit/nunit/issues/3919
/// </para>
/// <para>
/// Obviously <c>Console.WriteLine</c> is not code you would want in your app.
/// </para>
/// </remarks>
public sealed class TestLoggerAttribute : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        if (!parameter.ParameterType.IsGenericType
            || parameter.ParameterType.GetGenericTypeDefinition() != typeof(ILogger<>))
            throw new ArgumentException($"The parameter type must be an {nameof(ILogger)}<T>", nameof(parameter));
        
        return new TestLoggerCustomization(parameter.ParameterType.GenericTypeArguments.Single());
    }
}

public class TestLoggerCustomization : ICustomization
{
    static readonly MethodInfo customizeGenericMethod = typeof(TestLoggerCustomization).GetMethod(nameof(CustomizeGeneric), BindingFlags.Instance | BindingFlags.NonPublic)!;

    readonly Type loggerType;

    public void Customize(IFixture fixture)
    {
        customizeGenericMethod.MakeGenericMethod(loggerType).Invoke(this, [fixture]);
    }

    void CustomizeGeneric<T>(IFixture fixture)
    {
        fixture.Customize<ILogger<T>>(c => c.FromFactory(() => new TestContextLogger<T>()));
    }

    public TestLoggerCustomization(Type loggerType)
    {
        this.loggerType = loggerType ?? throw new ArgumentNullException(nameof(loggerType));
    }
}

public class TestContextLogger<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => Mock.Of<IDisposable>();

    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        TestContext.WriteLine($"{logLevel}: {formatter(state, exception)}{((exception is not null) ? ("\n" + exception.ToString()) : string.Empty)}");
    }
}