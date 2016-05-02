namespace Owin
{
    using System;
    using LimitsMiddleware;

#pragma warning disable 1591
    public static partial class AppBuilderExtensions
#pragma warning restore 1591
    {
        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder app, TimeSpan timeout, string loggerName = null)
        {
            app.MustNotNull("app");

            app.Use(Limits.ConnectionTimeout(timeout, loggerName));

            return app;
        }

        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getTimeout">
        ///     A delegate to retrieve the timeout timespan. Allows you
        ///     to supply different values at runtime.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder app, Func<TimeSpan> getTimeout,
            string loggerName = null)
        {
            app.MustNotNull("app");
            getTimeout.MustNotNull("getTimeout");

            app.Use(Limits.ConnectionTimeout(getTimeout, loggerName));

            return app;
        }


        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getTimeout">
        ///     A delegate to retrieve the timeout timespan. Allows you
        ///     to supply different values at runtime.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getTimeout</exception>
        public static IAppBuilder ConnectionTimeout(
            this IAppBuilder app,
            Func<RequestContext, TimeSpan> getTimeout,
            string loggerName = null)
        {
            app.MustNotNull("app");
            getTimeout.MustNotNull("getTimeout");

            app.Use(Limits.ConnectionTimeout(getTimeout, loggerName));

            return app;
        }
    }
}
