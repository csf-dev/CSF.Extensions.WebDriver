using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// A tuple of <see cref="Type"/>, one a concrete implementation of <see cref="IWebDriver"/>, the other
    /// the implementation of <see cref="DriverOptions"/> which that web driver type uses.
    /// </summary>
    public sealed class WebDriverAndOptionsTypePair : IEquatable<WebDriverAndOptionsTypePair>
    {
        /// <summary>
        /// Gets the web driver type.
        /// </summary>
        public Type WebDriverType { get; }

        /// <summary>
        /// Gets the options type.
        /// </summary>
        public Type OptionsType { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as WebDriverAndOptionsTypePair);

        /// <inheritdoc/>
        public override int GetHashCode() => WebDriverType.GetHashCode() ^ OptionsType.GetHashCode();

        /// <inheritdoc/>
        public bool Equals(WebDriverAndOptionsTypePair other)
        {
            if (ReferenceEquals(other, null)) return false;
            return other.WebDriverType == WebDriverType && other.OptionsType == OptionsType;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverAndOptionsTypePair"/>.
        /// </summary>
        /// <param name="webDriverType">The web driver type</param>
        /// <param name="optionsType">The options type</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverAndOptionsTypePair(Type webDriverType, Type optionsType)
        {
            WebDriverType = webDriverType ?? throw new ArgumentNullException(nameof(webDriverType));
            OptionsType = optionsType ?? throw new ArgumentNullException(nameof(optionsType));
        }
    }
}