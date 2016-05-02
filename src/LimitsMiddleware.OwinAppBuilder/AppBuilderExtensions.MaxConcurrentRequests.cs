namespace Owin
{
    using System;
    using LimitsMiddleware;

    public static partial class AppBuilderExtensions
    {
        /// <summary>
        ///     Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="maxConcurrentRequests">
        ///     The maximum number of concurrent requests. Use 0 or a negative
        ///     number to specify unlimited number of concurrent requests.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder app, int maxConcurrentRequests, string loggerName = null)
        {
            app.MustNotNull("app");

            return MaxConcurrentRequests(app, () => maxConcurrentRequests, loggerName);
        }

        /// <summary>
        ///     Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxConcurrentRequests">
        ///     A delegate to retrieve the maximum number of concurrent requests. Allows you
        ///     to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent
        ///     requests.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder app, Func<int> getMaxConcurrentRequests, string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxConcurrentRequests.MustNotNull("getMaxConcurrentRequests");

            app.Use(Limits.MaxConcurrentRequests(getMaxConcurrentRequests, loggerName));

            return app;
        }

        /// <summary>
        ///     Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxConcurrentRequests">
        ///     A delegate to retrieve the maximum number of concurrent requests. Allows you
        ///     to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent
        ///     requests.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder app,
            Func<RequestContext, int> getMaxConcurrentRequests, string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxConcurrentRequests.MustNotNull("getMaxConcurrentRequests");

            app.Use(Limits.MaxConcurrentRequests(getMaxConcurrentRequests, loggerName));

            return app;
        }
    }
}