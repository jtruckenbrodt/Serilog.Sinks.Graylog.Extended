using System;
using System.Collections.Generic;
using System.IO;

namespace Serilog.Sinks.GraylogGelf.Gelf
{
    /// <summary>
    /// Provides chunking capability for UDP transports as defined in the GELF specification.
    /// </summary>
    internal class GelfChunkEncoder
    {
        /// <summary>
        /// Contains the maximum size a single message chunk can be.
        /// If a message length (in byte) is greater than this value it must be chunked.
        /// </summary>
        public int MaxChunkSize => _maxChunkSize;
        private readonly int _maxChunkSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="GelfChunkEncoder"/> class.
        /// </summary>
        /// <param name="maxChunkSize">The maximum size a single chunk can have.</param>
        public GelfChunkEncoder(int maxChunkSize)
        {
            _maxChunkSize = maxChunkSize;
        }

        /// <summary>
        /// Translates a single message into multiple chunks if the input message size is greater than the defined <see cref="MaxChunkSize"/>.
        /// </summary>
        /// <param name="bytes">The encoded original message which should be chunked.</param>
        /// <returns>An sequential <see cref="IEnumerable{T}"/> of <see cref="Byte"/> arrays representing the cunked messages.</returns>
        public IEnumerable<byte[]> Encode(byte[] bytes)
        {
            if (bytes.Length <= _maxChunkSize)
                yield return bytes;
            else
            {
                var messageChunkSize = _maxChunkSize - GelfConstants.ChunkHeaderSize;
                var chunksCount = bytes.Length / messageChunkSize + 1;
                if(chunksCount > GelfConstants.MaxChunkCount)
                    throw new ArgumentOutOfRangeException("bytes");
                var remainingBytes = bytes.Length;
                var messageId = GenerateId();
                for (var chunkSequenceNumber = 0; chunkSequenceNumber < chunksCount; ++chunkSequenceNumber)
                {
                    var chunkOffset = chunkSequenceNumber * messageChunkSize;
                    var chunkBytes = Math.Min(messageChunkSize, remainingBytes);
                    using (var stream = new MemoryStream(messageChunkSize))
                    {
                        stream.WriteByte(0x1e);
                        stream.WriteByte(0x0f);
                        stream.Write(messageId, 0, messageId.Length);
                        stream.WriteByte((byte)chunkSequenceNumber);
                        stream.WriteByte((byte)chunksCount);
                        stream.Write(bytes, chunkOffset, chunkBytes);
                        yield return stream.ToArray();
                    }
                    remainingBytes -= chunkBytes;
                }
            }
        }
        
        private static byte[] GenerateId()
        {     
            //create a bit array to store the entire message id (which is 8 bytes)
            var ticks = (ulong)DateTime.UtcNow.Ticks;
            var result = BitConverter.GetBytes(ticks);
            for (var i = 1; i < 8; i++)
            {
                result[i] ^= result[i - 1];
            }
            return result;
        }
    }
}