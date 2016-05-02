namespace Owin
{
    using System;
    using LimitsMiddleware;

    public static partial class AppBuilderExtensions
    {
        /// <summary>
        ///     Limits the length of the query string.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder app, int maxQueryStringLength,
            string loggerName = null)
        {
            app.MustNotNull("app");

            return MaxQueryStringLength(app, () => maxQueryStringLength, loggerName);
        }

        /// <summary>
        ///     Limits the length of the query string.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder app, Func<int> getMaxQueryStringLength,
            string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxQueryStringLength.MustNotNull("getMaxQueryStringLength");

            app.Use(Limits.MaxQueryStringLength(getMaxQueryStringLength, loggerName));

            return app;
        }


        /// <summary>
        ///     Limits the length of the query string.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder app,
            Func<RequestContext, int> getMaxQueryStringLength, string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxQueryStringLength.MustNotNull("getMaxQueryStringLength");

            app.Use(Limits.MaxQueryStringLength(getMaxQueryStringLength, loggerName));

            return app;
        }
    }
}