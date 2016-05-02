namespace LimitsMiddleware
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using LimitsMiddleware.Logging;
    using Timer = System.Timers.Timer;

    internal class TimeoutStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly TimeSpan _timeout;
        private readonly ILog _logger;
        private readonly Timer _timer;

        public TimeoutStream(Stream innerStream, TimeSpan timeout, ILog logger)
        {
            _innerStream = innerStream;
            _timeout = timeout;
            _logger = logger;
            _timer = new Timer(_timeout.TotalMilliseconds)
            {
                AutoReset = false
            };
            _timer.Elapsed += (sender, args) =>
            {
                _logger.Info("Timeout of {0} reached.".FormatWith(_timeout));
                Close();
            };
            _timer.Start();
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get { return _innerStream.Position; }
            set { _innerStream.Position = value; }
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            _timer.Dispose();
            _innerStream.Close();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
            Reset();
            return read;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
            Reset();
        }

        private void Reset()
        {
            _timer.Stop();
            _logger.Debug("Timeout timer reseted.");
            _timer.Start();
        }
    }
}