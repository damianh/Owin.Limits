namespace Owin
{
    using System;
    using LimitsMiddleware;

#pragma warning disable 1591
    public static partial class AppBuilderExtensions
#pragma warning restore 1591
    {
        /// <summary>
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="maxContentLength">Maximum length of the content.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder app, int maxContentLength,
            string loggerName = null)
        {
            app.MustNotNull("app");

            return MaxRequestContentLength(app, () => maxContentLength, loggerName);
        }

        /// <summary>
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder app, Func<int> getMaxContentLength,
            string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxContentLength.MustNotNull("getMaxContentLength");

            app.Use(Limits.MaxRequestContentLength(getMaxContentLength, loggerName));

            return app;
        }

        /// <summary>
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder app,
            Func<RequestContext, int> getMaxContentLength, string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxContentLength.MustNotNull("getMaxContentLength");

            app.Use(Limits.MaxRequestContentLength(getMaxContentLength, loggerName));

            return app;
        }
    }
}