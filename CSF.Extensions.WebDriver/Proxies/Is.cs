using System;
using System.Reflection;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Utility/helper class to identify members.
    /// </summary>
    internal class Is
    {
        const string getterPrefix = "get_";

        /// <summary>
        /// Returns a value that indicates whether or not the <paramref name="method"/> is a property-getter
        /// for a property with the specified <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type (interface) upon which the property is defined</typeparam>
        /// <param name="name">The name of the property</param>
        /// <param name="method">The method info</param>
        /// <returns><see langword="true" /> if the <paramref name="method"/> is a getter for a property named
        /// <paramref name="name"/>, upon the type <typeparamref name="T"/>; <see langword="false" /> if not.</returns>
        internal static bool Getter<T>(string name, MethodInfo method) where T : class
            => method.DeclaringType == typeof(T) && method.Name == String.Concat(getterPrefix, name);
    }
}