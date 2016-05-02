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
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="maxContentLength">Maximum length of the content.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc MaxRequestContentLength(int maxContentLength, string loggerName = null)
        {
            return MaxRequestContentLength(() => maxContentLength, loggerName);
        }

        /// <summary>
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getMaxContentLength</exception>
        public static MidFunc MaxRequestContentLength(Func<int> getMaxContentLength, string loggerName = null)
        {
            getMaxContentLength.MustNotNull("getMaxContentLength");

            return MaxRequestContentLength(_ => getMaxContentLength(), loggerName);
        }

        /// <summary>
        ///     Limits the length of the request content.
        /// </summary>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <param name="loggerName">(Optional) The name of the logger log messages are written to.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        /// <exception cref="System.ArgumentNullException">getMaxContentLength</exception>
        public static MidFunc MaxRequestContentLength(
            Func<RequestContext, int> getMaxContentLength,
            string loggerName = null)
        {
            getMaxContentLength.MustNotNull("getMaxContentLength");

            loggerName = string.IsNullOrWhiteSpace(loggerName)
                ? "LimitsMiddleware.MaxRequestContentLength"
                : loggerName;
            var logger = LogProvider.GetLogger(loggerName);

            return
                next =>
                async env =>
                {
                    var context = new OwinContext(env);
                    var request = context.Request;
                    var requestMethod = request.Method.Trim().ToUpper();

                    if (requestMethod == "HEAD")
                    {
                        logger.Debug("HEAD request forwarded without check.");
                        await next(env);
                        return;
                    }
                    int maxContentLength = getMaxContentLength(new RequestContext(request));
                    logger.Debug("Max valid content length is {0}.".FormatWith(maxContentLength));
                    if (!IsChunkedRequest(request))
                    {
                        logger.Debug("Not a chunked request. Checking content length header.");
                        string contentLengthHeaderValue = request.Headers.Get("Content-Length");
                        if (contentLengthHeaderValue == null)
                        {
                            if (requestMethod == "PUT" || requestMethod == "POST")
                            {
                                SetResponseStatusCodeAndReasonPhrase(context, 411, "Length Required");
                                return;
                            }
                            request.Body = new ContentLengthLimitingStream(request.Body, 0);
                        }
                        else
                        {
                            int contentLength;
                            if (!int.TryParse(contentLengthHeaderValue, out contentLength))
                            {
                                logger.Info($"Invalid content length header value. Value: {contentLengthHeaderValue}");
                                SetResponseStatusCodeAndReasonPhrase(context, 400, "Bad Request");
                                return;
                            }
                            if (contentLength > maxContentLength)
                            {
                                logger.Info($"Content length of {contentLength} exceeds maximum of {maxContentLength}. " +
                                            "Request rejected.");
                                SetResponseStatusCodeAndReasonPhrase(context, 413, "Request Entity Too Large");
                                return;
                            }
                            logger.Debug("Content length header check passed.");

                            request.Body = new ContentLengthLimitingStream(request.Body, maxContentLength);
                            logger.Debug($"Request body stream configured with length limiting stream of {maxContentLength}.");
                        }
                    }
                    else
                    {
                        request.Body = new ContentLengthLimitingStream(request.Body, maxContentLength);
                        logger.Debug($"Request body stream configured with length limiting stream of {maxContentLength}.");
                        logger.Debug("Chunked request. Content length header not checked.");
                    }

                    try
                    {
                        logger.Debug("Request forwarded.");
                        await next(env);
                        logger.Debug("Processing finished.");
                    }
                    catch (ContentLengthRequiredException)
                    {
                        logger.Info("Content length required. Request canceled and rejected.");
                        SetResponseStatusCodeAndReasonPhrase(context, 411, "Length Required");
                    }
                    catch (ContentLengthExceededException)
                    {
                        logger.Info("Content length of {0} exceeded. Request canceled and rejected.".FormatWith(maxContentLength));
                        SetResponseStatusCodeAndReasonPhrase(context, 413, "Request Entity Too Large");
                    }

                };
        }

        private static bool IsChunkedRequest(IOwinRequest request)
        {
            string header = request.Headers.Get("Transfer-Encoding");
            return header != null && header.Equals("chunked", StringComparison.OrdinalIgnoreCase);
        }

        private static void SetResponseStatusCodeAndReasonPhrase(IOwinContext context, int statusCode, string reasonPhrase)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ReasonPhrase = reasonPhrase;
        }
    }
}
