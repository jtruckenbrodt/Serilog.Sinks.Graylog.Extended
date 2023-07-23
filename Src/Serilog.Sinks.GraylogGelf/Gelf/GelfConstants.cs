 using System;

 namespace Serilog.Sinks.GraylogGelf.Gelf
{
    /// <summary>
    /// Defines constants and default values used to generate GELF messages.
    /// </summary>
    internal static class GelfConstants
    {
        #region ----------------------- Properties ------------------------------------------------

        /// <summary>
        /// The default GELF version to set in each GELF message.
        /// Its value is <code>1.1</code>.
        /// </summary>
        public const string DefaultGelfVersion = "1.1";
        /// <summary>
        /// The maximum size a single UDP message could be (in bytes).
        /// Its value is <code>8192</code>.
        /// </summary>
        public const int MaxUdpMessageSize = 8192;
        /// <summary>
        /// The minimum size of a single UDP message to be exceeded before using compression (GZIP).
        /// Its value is <code>512</code>.
        /// </summary>
        public const int MinUdpMessageSizeForCompression = 512;
        /// <summary>
        /// The default amount of messages processed in one batch when using <see cref="PeriodicBatchingSink"/> and if not specified otherwise.
        /// </summary>
        /// <value>10</value>
        public const int DefaultBatchSize = 10;
        /// <summary>
        /// The size of the header used to identify the different chunks and their order, if the message has been chunked.
        /// </summary>
        public const int ChunkHeaderSize = 12;
        /// <summary>
        /// The maximum amount of chunks a single message con comprise. If a message will result in more chunks then all chunks more  than this amount will be dropped.
        /// </summary>
        public const int MaxChunkCount = 128;
        /// <summary>
        /// The default <see cref="TimeSpan"/> to wait before processing the next batch of messages 
        /// when using <see cref="PeriodicBatchingSink"/> and if not specified otherwise.
        /// </summary>
        /// <value>50 milliseconds</value>
        public static readonly TimeSpan DefaultBatchPeriod = TimeSpan.FromMilliseconds(50);

        #endregion
    }
}