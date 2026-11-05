
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CSF.Extensions.WebDriver
{
    /// <summary>
    /// Small service which provides a method for obtaining loggers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service is required in this library because we cannot use DI registration techniques to interrogate whether logging is
    /// already registered into DI.  This library is intended to be used alongside the BoDi DI container, which is part of the Reqnroll
    /// BDD framework. In this scenario, BoDi is consumed via an adpater, to make it behave a little like <see cref="IServiceCollection"/>.
    /// </para>
    /// <para>
    /// Unfortunately, because of limitations in BoDi's design, this is far from perfect (an LSP violation) and the resulting service
    /// collection throws from much of its functionality.  One of these is the indexer and <see cref="IEnumerable{T}.GetEnumerator()"/>
    /// implementation upon the service collection.  Those functions are the only viable way to determine whether logging has already
    /// been registered into DI (to avoid clobbering an existing DI registration), and they aren't available to us.
    /// </para>
    /// <para>
    /// This interface and its <c>internal</c> visibility impl provide a workaround to that, which decides at resolution time whether a
    /// logging implementation was formally registered into DI.  If it was then it is resolved from the container as usual.
    /// If it was not then a noop implementation of a logger is returned, to avoid throwing or returning <see langword="null"/>.
    /// </para>
    /// </remarks>
    public interface IGetsLogger
    {
        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type which intends to record log messages.</typeparam>
        /// <returns>A logger implementation</returns>
        ILogger<T> GetLogger<T>();
    }

    /// <summary>
    /// Internal-only implementation of <see cref="IGetsLogger"/>.
    /// </summary>
    class LogProvider : IGetsLogger
    {
        readonly IServiceProvider services;

        /// <inheritdoc/>
        public ILogger<T> GetLogger<T>() => services.GetService<ILogger<T>>() ?? new NullLogger<T>();

        /// <summary>
        /// Instantiates a new log provider instance.
        /// </summary>
        /// <param name="services">The service provider</param>
        /// <exception cref="ArgumentNullException">If the service provider is <see langword="null"/></exception>
        public LogProvider(IServiceProvider services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}