namespace LimitsMiddleware
{
    using System;
    using System.IO;
    using LimitsMiddleware.LibOwin;
    using LimitsMiddleware.Logging;
    using MidFunc = System.Func<
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
       >;

    public static partial class Limits
    {
        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc ConnectionTimeout(TimeSpan timeout, string loggerName = null)
        {
            timeout.MustNotNull("options");

            return ConnectionTimeout(() => timeout, loggerName);
        }

        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="getTimeout">
        ///     A delegate to retrieve the timeout timespan. Allows you
        ///     to supply different values at runtime.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getTimeout</exception>
        public static MidFunc ConnectionTimeout(Func<TimeSpan> getTimeout, string loggerName = null)
        {
            getTimeout.MustNotNull("getTimeout");

            return ConnectionTimeout(_ => getTimeout(), loggerName);
        }

        /// <summary>
        ///     Timeouts the connection if there hasn't been an read activity on the request body stream or any
        ///     write activity on the response body stream.
        /// </summary>
        /// <param name="getTimeout">
        ///     A delegate to retrieve the timeout timespan. Allows you
        ///     to supply different values at runtime.
        /// </param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getTimeout</exception>
        public static MidFunc ConnectionTimeout(
            Func<RequestContext, TimeSpan> getTimeout,
            string loggerName = null)
        {
            getTimeout.MustNotNull("getTimeout");

            loggerName = string.IsNullOrWhiteSpace(loggerName)
                ? "LimitsMiddleware.ConnectionTimeout"
                : loggerName;

            var logger = LogProvider.GetLogger(loggerName);

            return
                next =>
                env =>
                {
                    var context = new OwinContext(env);
                    var limitsRequestContext = new RequestContext(context.Request);

                    var requestBodyStream = context.Request.Body ?? Stream.Null;
                    var responseBodyStream = context.Response.Body;

                    var connectionTimeout = getTimeout(limitsRequestContext);
                    context.Request.Body = new TimeoutStream(requestBodyStream, connectionTimeout, logger);
                    context.Response.Body = new TimeoutStream(responseBodyStream, connectionTimeout, logger);
                    return next(env);
                };
        }
    }
}
