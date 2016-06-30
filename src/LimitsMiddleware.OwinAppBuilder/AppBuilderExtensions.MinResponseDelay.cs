namespace Owin
{
    using System;
    using LimitsMiddleware;

    public static partial class AppBuilderExtensions
    {
        /// <summary>
        ///     Sets a minimum delay in miliseconds before sending the response.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="minDelay">
        ///     The minimum delay to wait before sending the response.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MinResponseDelay(this IAppBuilder app, int minDelay, string loggerName = null)
        {
            app.MustNotNull("app");

            return MinResponseDelay(app, () => minDelay, loggerName);
        }

        /// <summary>
        ///     Sets a minimum delay before sending the response.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMinDelay">
        ///     A delegate to retrieve the maximum number of bytes per second to be transferred.
        ///     Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The app instance.</returns>
        public static IAppBuilder MinResponseDelay(this IAppBuilder app, Func<int> getMinDelay, string loggerName = null)
        {
            app.MustNotNull("app");

            app.Use(Limits.MinResponseDelay(getMinDelay, loggerName));
            return app;
        }

        /// <summary>
        ///     Sets a minimum delay before sending the response.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMinDelay">
        ///     A delegate to retrieve the minimum delay before calling the next stage in the pipeline. Note:
        ///     the delegate should return quickly.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>The app instance.</returns>
        public static IAppBuilder MinResponseDelay(this IAppBuilder app, Func<TimeSpan> getMinDelay,
            string loggerName = null)
        {
            app.MustNotNull("app");

            app.Use(Limits.MinResponseDelay(getMinDelay, loggerName));
            return app;
        }

        /// <summary>
        ///     Sets a minimum delay before sending the response.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMinDelay">
        ///     A delegate to retrieve the minimum delay before calling the next stage in the pipeline. Note:
        ///     the delegate should return quickly.
        /// </param>
        /// <returns>The app instance.</returns>
        public static IAppBuilder MinResponseDelay(this IAppBuilder app, Func<RequestContext, TimeSpan> getMinDelay,
            string loggerName = null)
        {
            app.MustNotNull("app");

            app.Use(Limits.MinResponseDelay(getMinDelay, loggerName));
            return app;
        }
    }
}
