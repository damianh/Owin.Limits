namespace Owin
{
    using System;
    using LimitsMiddleware;

    public static partial class AppBuilderExtensions
    {
        /// <summary>
        ///     Limits the length of the URL.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxUrlLength(this IAppBuilder app, int maxUrlLength, string loggerName = null)
        {
            app.MustNotNull("app");

            return MaxUrlLength(app, () => maxUrlLength, loggerName);
        }

        /// <summary>
        ///     Limits the length of the URL.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxUrlLength(this IAppBuilder app, Func<int> getMaxUrlLength, string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxUrlLength.MustNotNull("getMaxUrlLength");

            app.Use(Limits.MaxUrlLength(getMaxUrlLength, loggerName));
            return app;
        }

        /// <summary>
        ///     Limits the length of the URL.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxUrlLength(this IAppBuilder app, Func<RequestContext, int> getMaxUrlLength,
            string loggerName = null)
        {
            app.MustNotNull("app");
            getMaxUrlLength.MustNotNull("getMaxUrlLength");

            app.Use(Limits.MaxUrlLength(getMaxUrlLength, loggerName));
            return app;
        }
    }
}