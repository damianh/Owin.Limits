namespace LimitsMiddleware
{
    using System;
    using LimitsMiddleware.LibOwin;
    using LimitsMiddleware.Logging;
    using MidFunc = System.Func<
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
       >;

    public static partial class Limits
    {
        /// <summary>
        ///     Limits the length of the query string.
        /// </summary>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc MaxQueryStringLength(int maxQueryStringLength, string loggerName = null)
        {
            return MaxQueryStringLength(_ => maxQueryStringLength, loggerName);
        }

        /// <summary>
        ///     Limits the length of the query string.
        /// </summary>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc MaxQueryStringLength(Func<int> getMaxQueryStringLength, string loggerName = null)
        {
            return MaxQueryStringLength(_ => getMaxQueryStringLength(), loggerName);
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getMaxQueryStringLength</exception>
        public static MidFunc MaxQueryStringLength(
            Func<RequestContext, int> getMaxQueryStringLength,
            string loggerName = null)
        {
            getMaxQueryStringLength.MustNotNull("getMaxQueryStringLength");
            loggerName = string.IsNullOrWhiteSpace(loggerName)
                ? "LimitsMiddleware.MaxQueryStringLength"
                : loggerName;
            var logger = LogProvider.GetLogger(loggerName);

            return
                next =>
                async env =>
                {
                    var context = new OwinContext(env);
                    var requestContext = new RequestContext(context.Request);

                    QueryString queryString = context.Request.QueryString;
                    if (queryString.HasValue)
                    {
                        int maxQueryStringLength = getMaxQueryStringLength(requestContext);
                        string unescapedQueryString = Uri.UnescapeDataString(queryString.Value);
                        logger.Debug("Querystring of request with an unescaped length of {0}".FormatWith(unescapedQueryString.Length));
                        if (unescapedQueryString.Length > maxQueryStringLength)
                        {
                            logger.Info("Querystring (Length {0}) too long (allowed {1}). Request rejected.".FormatWith(
                                unescapedQueryString.Length,
                                maxQueryStringLength));
                            context.Response.StatusCode = 414;
                            context.Response.ReasonPhrase = "Request-URI Too Large";
                            context.Response.Write(context.Response.ReasonPhrase);
                            return;
                        }
                    }
                    else
                    {
                        logger.Debug("No querystring.");
                    }
                    await next(env);
                };
        }
    }
}
