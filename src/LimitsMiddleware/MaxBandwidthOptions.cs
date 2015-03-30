namespace LimitsMiddleware
{
    using System;

    /// <summary>
    /// Options for limiting the max bandwidth.
    /// </summary>
    public class MaxBandwidthOptions : OptionsBase
    {
        private readonly Func<RequestContext, int> _getMaxBytesPerSecond;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxBandwidthOptions"/> class.
        /// </summary>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        public MaxBandwidthOptions(int maxBytesPerSecond) : this(() => maxBytesPerSecond)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxBandwidthOptions"/> class.
        /// </summary>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        public MaxBandwidthOptions(Func<int> getMaxBytesPerSecond)
            : this(_ => getMaxBytesPerSecond())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxBandwidthOptions"/> class.
        /// </summary>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred,
        /// based on the <see cref="RequestContext"/>.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        public MaxBandwidthOptions(Func<RequestContext, int> getMaxBytesPerSecond)
        {
            getMaxBytesPerSecond.MustNotNull("getMaxBytesPerSecond");

            _getMaxBytesPerSecond = getMaxBytesPerSecond;
        }

        /// <summary>
        /// The maximum bytes per second
        /// </summary>
        [Obsolete("Use GetMaxBytesPerSecond instead.", true)]
        public int MaxBytesPerSecond
        {
            get { throw new NotSupportedException(); }
        }

        public int GetMaxBytesPerSecond(RequestContext requestContext)
        {
            return _getMaxBytesPerSecond(requestContext);
        }
    }
}