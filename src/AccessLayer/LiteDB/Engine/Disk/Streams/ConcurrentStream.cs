using System;
using System.IO;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Implement internal thread-safe Stream using lock control - A single instance of ConcurrentStream are not multi thread,
    ///     but multiples ConcurrentStream instances using same stream base will support concurrency
    /// </summary>
    internal class ConcurrentStream : Stream
    {
        public ConcurrentStream(Stream stream, bool canWrite)
        {
            _stream = stream;
            _canWrite = canWrite;
        }

        #region Fields and Autoproperties

        private readonly bool _canWrite;
        private readonly Stream _stream;

        private long _position;

        #endregion

        /// <inheritdoc />
        public override bool CanRead => _stream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _stream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => _canWrite;

        /// <inheritdoc />
        public override long Length => _stream.Length;

        /// <inheritdoc />
        public override long Position
        {
            get => _position;
            set => _position = value;
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            // lock internal stream and set position before read
            lock (_stream)
            {
                _stream.Position = _position;
                var read = _stream.Read(buffer, offset, count);
                _position = _stream.Position;
                return read;
            }
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_stream)
            {
                var position = origin == SeekOrigin.Begin
                    ? offset
                    : origin == SeekOrigin.Current
                        ? _position + offset
                        : _position - offset;

                _position = position;

                return _position;
            }
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_canWrite == false)
            {
                throw new NotSupportedException("Current stream are readonly");
            }

            // lock internal stream and set position before write
            lock (_stream)
            {
                _stream.Position = _position;
                _stream.Write(buffer, offset, count);
                _position = _stream.Position;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _stream.Dispose();
        }
    }
}
