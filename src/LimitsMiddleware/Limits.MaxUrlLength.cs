namespace LimitsMiddleware
{
    using System;
    using System.Threading.Tasks;
    using LimitsMiddleware.LibOwin;
    using LimitsMiddleware.Logging;
    using MidFunc = System.Func<
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
       System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
       >;

    public static partial class Limits
    {
        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc MaxUrlLength(int maxUrlLength, string loggerName = null)
        {
            return MaxUrlLength(() => maxUrlLength, loggerName);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="getMaxUrlLength">Maximum length of the URL.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc MaxUrlLength(Func<int> getMaxUrlLength, string loggerName = null)
        {
            return MaxUrlLength(_ => getMaxUrlLength(), loggerName);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getMaxUrlLength</exception>
        public static MidFunc MaxUrlLength(Func<RequestContext, int> getMaxUrlLength, string loggerName = null)
        {
            getMaxUrlLength.MustNotNull("getMaxUrlLength");

            loggerName = string.IsNullOrWhiteSpace(loggerName)
                ? "LimitsMiddleware.MaxUrlLength"
                : loggerName;
            var logger = LogProvider.GetLogger(loggerName);

            return
                next =>
                env =>
                {
                    var context = new OwinContext(env);
                    int maxUrlLength = getMaxUrlLength(new RequestContext(context.Request));
                    string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

                    logger.Debug("Checking request url length.");
                    if (unescapedUri.Length > maxUrlLength)
                    {
                        logger.Info(
                            "Url \"{0}\"(Length: {2}) exceeds allowed length of {1}. Request rejected.".FormatWith(
                            unescapedUri,
                            maxUrlLength,
                            unescapedUri.Length));
                        context.Response.StatusCode = 414;
                        context.Response.ReasonPhrase = "Request-URI Too Large";
                        context.Response.Write(context.Response.ReasonPhrase);
                        return Task.FromResult(0);
                    }
                    logger.Debug("Check passed. Request forwarded.");
                    return next(env);
                };
        }

    }
}
