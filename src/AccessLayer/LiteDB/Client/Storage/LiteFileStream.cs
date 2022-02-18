using System;
using System.IO;
using static LiteDB.Constants;

namespace LiteDB
{
    public partial class LiteFileStream<TFileId> : Stream
    {
        #region Constants and Static Readonly

        /// <summary>
        ///     Number of bytes on each chunk document to store
        /// </summary>
        public const int MAX_CHUNK_SIZE = 255 * 1024; // 255kb like GridFS

        #endregion

        internal LiteFileStream(
            ILiteCollection<LiteFileInfo<TFileId>> files,
            ILiteCollection<BsonDocument> chunks,
            LiteFileInfo<TFileId> file,
            BsonValue fileId,
            FileAccess mode)
        {
            _files = files;
            _chunks = chunks;
            _file = file;
            _fileId = fileId;
            _mode = mode;

            if (mode == FileAccess.Read)
            {
                // initialize first data block
                _currentChunkData = GetChunkData(_currentChunkIndex);
            }
            else if (mode == FileAccess.Write)
            {
                _buffer = new MemoryStream(MAX_CHUNK_SIZE);

                if (_file.Length > 0)
                {
                    // delete all chunks before re-write
                    var count = _chunks.DeleteMany(
                        "_id BETWEEN { f: @0, n: 0 } AND { f: @0, n: 99999999 }",
                        _fileId
                    );

                    ENSURE(count == _file.Chunks);

                    // clear file content length+chunks
                    _file.Length = 0;
                    _file.Chunks = 0;
                }
            }
        }

        #region Fields and Autoproperties

        private readonly BsonValue _fileId;
        private readonly FileAccess _mode;
        private readonly ILiteCollection<BsonDocument> _chunks;

        private readonly ILiteCollection<LiteFileInfo<TFileId>> _files;
        private readonly LiteFileInfo<TFileId> _file;
        private byte[] _currentChunkData;
        private int _currentChunkIndex;
        private int _positionInChunk;

        private long _streamPosition;
        private MemoryStream _buffer;

        #endregion

        /// <inheritdoc />
        public override bool CanRead => _mode == FileAccess.Read;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => _mode == FileAccess.Write;

        /// <inheritdoc />
        public override long Length => _file.Length;

        /// <inheritdoc />
        public override long Position
        {
            get => _streamPosition;
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Get file information
        /// </summary>
        public LiteFileInfo<TFileId> FileInfo => _file;

        #region Dispose

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_disposed)
            {
                return;
            }

            if (disposing && CanWrite)
            {
                Flush();
                _buffer?.Dispose();
            }

            _disposed = true;
        }

        #endregion

        #region Not supported operations

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
