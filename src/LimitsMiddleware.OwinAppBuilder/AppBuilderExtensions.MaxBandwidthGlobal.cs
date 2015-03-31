﻿namespace Owin
{
    using System;
    using LimitsMiddleware;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using MidFunc = System.Func<
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
        >;

    public static partial class AppBuilderExtensions
    {
        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        /// <returns>The IAppBuilder instance.</returns>
        public static IAppBuilder MaxBandwidthGlobal(this IAppBuilder app, int maxBytesPerSecond)
        {
            app.MustNotNull("app");

            return MaxBandwidthGlobal(app, () => maxBytesPerSecond);
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        /// <returns>The app instance.</returns>
        public static IAppBuilder MaxBandwidthGlobal(this IAppBuilder app, Func<int> getMaxBytesPerSecond)
        {
            app.MustNotNull("app");

            return MaxBandwidthGlobal(app, new MaxBandwidthGlobalOptions(getMaxBytesPerSecond));
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder instance.</param>
        /// <param name="perRequestOptions">The max bandwith options.</param>
        /// <returns>The IAppBuilder instance.</returns>
        /// <exception cref="System.ArgumentNullException">app</exception>
        public static IAppBuilder MaxBandwidthGlobal(this IAppBuilder app, MaxBandwidthGlobalOptions perRequestOptions)
        {
            app.MustNotNull("app");
            perRequestOptions.MustNotNull("options");

            app.Use(Limits.MaxBandwidthGlobal(perRequestOptions));
            return app;
        }
    }
}
