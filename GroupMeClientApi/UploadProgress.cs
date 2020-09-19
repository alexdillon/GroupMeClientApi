using System;

namespace GroupMeClientApi
{
    /// <summary>
    /// <see cref="UploadProgress"/> allows for monitoring the asychronous upload progress of an API operation.
    /// </summary>
    public class UploadProgress
    {
        private int bytesUploaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadProgress"/> class.
        /// </summary>
        public UploadProgress()
        {
        }

        /// <summary>
        /// An event that fires when the number of bytes that have been uploaded changes.
        /// </summary>
        public event Action<BytesUploadedChangedEventArgs> BytesUploadedChanged;

        /// <summary>
        /// <see cref="UploadState"/> defines the various stages an upload operation can be in.
        /// </summary>
        public enum UploadState
        {
            /// <summary>
            /// The upload has not yet begun.
            /// </summary>
            Waiting,

            /// <summary>
            /// The content is actively being uploaded.
            /// </summary>
            Uploading,

            /// <summary>
            /// The upload operation has completed.
            /// </summary>
            Completed,
        }

        /// <summary>
        /// Gets the state of the current upload operation.
        /// </summary>
        public UploadState State { get; internal set; } = UploadState.Waiting;

        /// <summary>
        /// Gets the number of bytes that have been successfully uploaded.
        /// </summary>
        public int BytesUploaded
        {
            get => this.bytesUploaded;

            internal set
            {
                if (this.bytesUploaded != value)
                {
                    this.bytesUploaded = value;
                    this.BytesUploadedChanged?.Invoke(new BytesUploadedChangedEventArgs() { BytesUploaded = value });
                }
            }
        }

        /// <summary>
        /// <see cref="BytesUploadedChangedEventArgs"/> provides event arguments for an upload change event.
        /// </summary>
        public class BytesUploadedChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the number of bytes that have been successfully uploaded.
            /// </summary>
            public int BytesUploaded { get; internal set; }
        }
    }
}
