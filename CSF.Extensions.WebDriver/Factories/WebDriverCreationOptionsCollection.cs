using System;
using System.Collections.Generic;

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
        /// <see cref="WebDriverFactoryExtensions.GetWebDriver(ICreatesWebDriverFromOptions, WebDriverCreationOptionsCollection)"/> in
        /// order to get a WebDriver from the currently selected configuration.
        /// </para>
        /// </remarks>
        public string SelectedConfiguration { get; set; }
    }
}