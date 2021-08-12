using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing an image.
    /// </summary>
    public class FileAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachment"/> class.
        /// </summary>
        internal FileAttachment()
        {
        }

        /// <summary>
        /// Gets the default buffer size, in bytes, used for uploading content.
        /// </summary>
        public static int DefaultUploadBlockSize => ProgressableBlockContent.DefaultBufferSize;

        /// <inheritdoc/>
        public override string Type { get; } = "file";

        /// <summary>
        /// Gets the URL of the image attachment.
        /// </summary>
        [JsonProperty("file_id")]
        public string Id { get; internal set; }

        private static TimeSpan UploadPollTime => TimeSpan.FromSeconds(1);

        /// <summary>
        /// Uploads a document to GroupMe and returns the created <see cref="FileAttachment"/>.
        /// </summary>
        /// <param name="filename">The name of the file being sent.</param>
        /// <param name="document">The document to upload.</param>
        /// <param name="messageContainer">The <see cref="IMessageContainer"/> that the message is being sent to.</param>
        /// <param name="cancellationTokenSource">The cancellation source to use for the upload operation.</param>
        /// <param name="uploadProgress">A monitor that will receive progress updates for the file upload operation.</param>
        /// <returns>An <see cref="ImageAttachment"/> if uploaded successfully, null otherwise.</returns>
        public static async Task<FileAttachment> CreateFileAttachment(string filename, byte[] document, IMessageContainer messageContainer, CancellationTokenSource cancellationTokenSource = null, UploadProgress uploadProgress = null)
        {
            cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            uploadProgress = uploadProgress ?? new UploadProgress();

            var encodedFilename = System.Web.HttpUtility.UrlEncode(filename);
            var url = GetGroupMeFileApiBaseUrl(messageContainer.Messages.First()) + $"/files?name={encodedFilename}";
            var mimeType = GroupMeDocumentMimeTypeMapper.ExtensionToMimeType(System.IO.Path.GetExtension(filename));

            var restResponse = await messageContainer.Client.ExecuteRestRequestAsync(url, document, mimeType, cancellationTokenSource.Token, uploadProgress);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var result = JsonConvert.DeserializeObject<FileUploadResponse>(await restResponse.Content.ReadAsStringAsync());
                var finishedFileId = string.Empty;
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(UploadPollTime);
                    var status = await CheckUploadStatus(result.StatusUrl, messageContainer, cancellationTokenSource);
                    if (status != null && status.Status == FileUploadStatusResponse.CompletedStatus)
                    {
                        finishedFileId = status.FileId;
                        break;
                    }
                }

                return new FileAttachment()
                {
                    Id = finishedFileId,
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves information about this <see cref="FileAttachment"/> from GroupMe.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> that the file is contained in.</param>
        /// <returns>An <see cref="FileData"/> if data is retrieved successfully, null otherwise.</returns>
        public async Task<FileData> GetFileData(Message message)
        {
            var messageContainer = (IMessageContainer)message.Group ?? (IMessageContainer)message.Chat;
            var request = messageContainer.Client.CreateRawRestRequest(FileAttachment.GetGroupMeFileDataApiUrl(message), RestSharp.Method.POST);

            var payload = new
            {
                file_ids = new string[] { this.Id },
            };

            request.AddJsonBody(payload);

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await messageContainer.Client.ExecuteRestRequestAsync(request, cancellationTokenSource.Token);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var results = JsonConvert.DeserializeObject<List<FileDataResponse>>(restResponse.Content);
                var result = results.First();
                return result.FileData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a download URL for this <see cref="FileAttachment"/>.
        /// </summary>
        /// <param name="message">The message that contains the <see cref="FileAttachment"/>.</param>
        /// <returns>A download URL string.</returns>
        public async Task<byte[]> DownloadFileAsync(Message message)
        {
            var url = GetGroupMeFileApiBaseUrl(message) + $"/files/{this.Id}";
            var messageContainer = (IMessageContainer)message.Group ?? message.Chat;
            var request = messageContainer.Client.CreateRawRestRequest(url, RestSharp.Method.POST);

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await messageContainer.Client.ExecuteRestRequestAsync(request, cancellationTokenSource.Token);

            return restResponse.RawBytes;
        }

        private static string GetGroupMeFileDataApiUrl(Message message)
        {
            return GetGroupMeFileApiBaseUrl(message) + "/fileData";
        }

        private static string GetGroupMeFileApiBaseUrl(Message message)
        {
            var id = string.IsNullOrEmpty(message.ConversationId) ? message.Group.Id : message.ConversationId;
            return $"https://file.groupme.com/v1/{id}";
        }

        private static async Task<FileUploadStatusResponse> CheckUploadStatus(string checkUrl, IMessageContainer messageContainer, CancellationTokenSource cancellationTokenSource)
        {
            var request = messageContainer.Client.CreateRawRestRequest(checkUrl, RestSharp.Method.GET);
            var restResponse = await messageContainer.Client.ExecuteRestRequestAsync(request, cancellationTokenSource.Token);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<FileUploadStatusResponse>(restResponse.Content);
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// <see cref="FileData"/> provides information about a GroupMe <see cref="FileAttachment"/>.
        /// </summary>
        public class FileData
        {
            /// <summary>
            /// Gets the filename.
            /// </summary>
            [JsonProperty("file_name")]
            public string FileName { get; internal set; }

            /// <summary>
            /// Gets the size of the file in bytes.
            /// </summary>
            [JsonProperty("file_size")]
            public string FileSize { get; internal set; }

            /// <summary>
            /// Gets the mime type of the file.
            /// </summary>
            [JsonProperty("mime_type")]
            public string MimeType { get; internal set; }
        }

        /// <summary>
        /// <see cref="GroupMeDocumentMimeTypeMapper"/> provides mappings between supported GroupMe Document Mime Types and Extensions.
        /// </summary>
        public class GroupMeDocumentMimeTypeMapper
        {
            private static readonly Dictionary<string, string> MimeMap = new Dictionary<string, string>()
            {
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "	application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".ppt", "	application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".pdf", "application/pdf" },
                { ".psd", "image/vnd.adobe.photoshop" },
                { ".ai", "application/postscript" },
            };

            /// <summary>
            /// Gets a listing of supported extension for GroupMe Document Attachments.
            /// </summary>
            public static IEnumerable<string> SupportedExtensions
            {
                get
                {
                    foreach (var key in MimeMap.Keys)
                    {
                        yield return key;
                    }
                }
            }

            /// <summary>
            /// Returns the correct file extension for a given mime type.
            /// </summary>
            /// <param name="mimeType">The mime type to lookup.</param>
            /// <returns>A file extension.</returns>
            public static string MimeTypeToExtension(string mimeType)
            {
                var extension = MimeMap.FirstOrDefault(x => x.Value == mimeType).Key;
                return extension;
            }

            /// <summary>
            /// Returns the correct mime type for a given file extension.
            /// </summary>
            /// <param name="extension">The extension to lookup.</param>
            /// <returns>A mime type.</returns>
            public static string ExtensionToMimeType(string extension)
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                if (MimeMap.TryGetValue(extension, out var mimeType))
                {
                    return mimeType;
                }
                else
                {
                    return "application/octet-stream";
                }
            }

            /// <summary>
            /// Returns a value indicating whether a specific extension is supported by GroupMe's Document service.
            /// </summary>
            /// <param name="extension">A file extension.</param>
            /// <returns>A boolean value indicating whether the provided extension is supported.</returns>
            public bool IsExtensionSupported(string extension)
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                return MimeMap.TryGetValue(extension, out var _);
            }
        }

        private class FileDataResponse
        {
            [JsonProperty("file_id")]
            public string FileId { get; internal set; }

            [JsonProperty("meta")]
            public string ResponseStatus { get; internal set; }

            [JsonProperty("file_data")]
            public FileData FileData { get; internal set; }
        }

        private class FileUploadResponse
        {
            [JsonProperty("status_url")]
            public string StatusUrl { get; internal set; }
        }

        private class FileUploadStatusResponse
        {
            public const string CompletedStatus = "completed";

            [JsonProperty("status")]
            public string Status { get; internal set; }

            [JsonProperty("file_id")]
            public string FileId { get; internal set; }
        }
    }
}