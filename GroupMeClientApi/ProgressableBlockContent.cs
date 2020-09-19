using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GroupMeClientApi
{
    /// <summary>
    /// <see cref="ProgressableBlockContent"/> provides an implementation of <see cref="HttpContent"/> that allows for uploading a large byte array while reporting progress.
    /// </summary>
    internal class ProgressableBlockContent : HttpContent
    {
        /// <summary>
        /// The default buffer size used for uploading block content.
        /// </summary>
        public const int DefaultBufferSize = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressableBlockContent"/> class.
        /// </summary>
        /// <param name="content">The block of content to upload.</param>
        /// <param name="progress">A monitor that will receive upload progress updates.</param>
        public ProgressableBlockContent(byte[] content, UploadProgress progress)
            : this(content, DefaultBufferSize, progress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressableBlockContent"/> class.
        /// </summary>
        /// <param name="content">The block of content to upload.</param>
        /// <param name="bufferSize">The buffer size used for uploads.</param>
        /// <param name="progress">A monitor that will receive upload progress updates.</param>
        public ProgressableBlockContent(byte[] content, int bufferSize, UploadProgress progress)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            this.Content = content ?? throw new ArgumentNullException(nameof(content));
            this.BufferSize = bufferSize;
            this.UploadProgress = progress;
        }

        private byte[] Content { get; }

        private int BufferSize { get; }

        private UploadProgress UploadProgress { get; }

        /// <inheritdoc/>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);

            return Task.Run(() =>
            {
                this.UploadProgress.State = UploadProgress.UploadState.Waiting;

                var uploaded = 0;
                var size = this.Content.Length;

                while (uploaded < size)
                {
                    var remaining = size - uploaded;
                    var block = Math.Min(remaining, this.BufferSize);

                    stream.Write(this.Content, uploaded, block);
                    uploaded += block;

                    this.UploadProgress.BytesUploaded = uploaded;
                    this.UploadProgress.State = UploadProgress.UploadState.Uploading;
                }

                this.UploadProgress.State = UploadProgress.UploadState.Completed;
            });
        }

        /// <inheritdoc/>
        protected override bool TryComputeLength(out long length)
        {
            length = this.Content.Length;
            return true;
        }
    }
}
