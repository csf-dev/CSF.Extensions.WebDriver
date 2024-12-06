using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Describes a collection of configurations which may be used with <see cref="ICreatesWebDriverFromOptions"/>
    /// to get a WebDriver.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This object additionally has a property that indicates which of those configurations is 'currently selected': <see cref="SelectedConfiguration"/>.
    /// This enables techniques in which many WebDriver configurations are available, such as in an <c>appsettings.json</c>
    /// file and a parameter is used to choose between them by name.
    /// Such a value might be provided via a command-line argument, for example.
    /// </para>
    /// </remarks>
    public class WebDriverCreationOptionsCollection
    {
        IDictionary<string,WebDriverCreationOptions> driverConfigurations = new Dictionary<string,WebDriverCreationOptions>();

        /// <summary>
        /// Gets or sets a name/value collection of <see cref="WebDriverCreationOptions"/> instances,
        /// containing all of the WebDriver configurations which are available.
        /// </summary>
        public IDictionary<string,WebDriverCreationOptions> DriverConfigurations
        {
            get => driverConfigurations;
            set => driverConfigurations = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the key of a WebDriver configuration which is currently selected.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, this value must correspond to a key of the collection <see cref="DriverConfigurations"/>.
        /// If appropriately set, then the current <see cref="WebDriverCreationOptionsCollection"/> instance may be used as the parameter to
        /// <see cref="WebDriverFactoryExtensions.GetWebDriver(ICreatesWebDriverFromOptions, WebDriverCreationOptionsCollection,Action{OpenQA.Selenium.DriverOptions})"/> in
        /// order to get a WebDriver from the currently selected configuration.
        /// </para>
        /// </remarks>
        public string SelectedConfiguration { get; set; }

        /// <summary>
        /// Gets the <see cref="WebDriverCreationOptions"/> from the <see cref="DriverConfigurations"/> which is considered 'currently selected'.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There are two ways in which an item in <see cref="DriverConfigurations"/> can be the currently-selected one:
        /// </para>
        /// <list type="bullet">
        /// <item><description>The <see cref="SelectedConfiguration"/> is non-<see langword="null" /> and not an empty
        /// string.  The driver configuration with a key matching the name of the selected configuration is the currently selected one.</description></item>
        /// <item><description>If <see cref="SelectedConfiguration"/> is null or empty but there is precisely one item present within
        /// <see cref="DriverConfigurations"/>; that single configuration is considered to be currently selected.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>A <see cref="WebDriverCreationOptions"/></returns>
        /// <exception cref="InvalidOperationException">If <see cref="SelectedConfiguration"/> is not <see langword="null" /> or empty, but
        /// there is no item within <see cref="DriverConfigurations"/> with a key matching the selected configuration
        /// or if <see cref="SelectedConfiguration"/> is either <see langword="null" /> or empty and there is not precisely one configuration
        /// within <see cref="DriverConfigurations"/>.</exception>
        public WebDriverCreationOptions GetSelectedConfiguration()
        {
            if (string.IsNullOrEmpty(SelectedConfiguration))
            {
                return DriverConfigurations.Count == 1
                    ? DriverConfigurations.Single().Value
                    : throw new InvalidOperationException($"If the {nameof(SelectedConfiguration)} is not set then there must be precisely one configuration present in {nameof(DriverConfigurations)}.");
            }

            if(driverConfigurations.TryGetValue(SelectedConfiguration, out var options)) return options;
            throw new InvalidOperationException($"The {nameof(SelectedConfiguration)}: '{SelectedConfiguration}' must exist as a key within the {nameof(DriverConfigurations)}.");
        }
    }
}