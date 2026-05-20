using Microsoft.Extensions.Configuration;

namespace CSF.Extensions.WebDriver;

public static class ConfigurationFactory
{
    /// <summary>
    /// Helper method to create an <see cref="IConfiguration"/> from a specified JSON string.
    /// </summary>
    /// <param name="jsonConfig">A JSON string which will be used as the basis for the returned config.</param>
    /// <returns>A task exposing a configuration object, created from the JSON string.</returns>
    public static async Task<IConfiguration> GetConfigurationAsync(string jsonConfig)
    {
        var builder = new ConfigurationBuilder();

        var stream = new MemoryStream ();
        using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(jsonConfig);
        await writer.FlushAsync();
        stream.Position = 0;

        builder.AddJsonStream(stream);
        return builder.Build();
    }
}