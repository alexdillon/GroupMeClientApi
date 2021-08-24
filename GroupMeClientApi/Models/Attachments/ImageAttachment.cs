using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing an image.
    /// </summary>
    public class ImageAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAttachment"/> class.
        /// </summary>
        internal ImageAttachment()
        {
        }

        /// <summary>
        /// Gets the default buffer size, in bytes, used for uploading content.
        /// </summary>
        public static int DefaultUploadBlockSize => ProgressableBlockContent.DefaultBufferSize;

        /// <summary>
        /// Gets a listing of supported extension for GroupMe Image Attachments.
        /// </summary>
        public static IEnumerable<string> SupportedExtensions { get; } = new List<string>() { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        /// <inheritdoc/>
        public override string Type { get; } = "image";

        /// <summary>
        /// Gets the URL of the image attachment.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; internal set; }

        private static string GroupMeImageApiUrl => "https://image.groupme.com/pictures";

        /// <summary>
        /// Uploads an image to GroupMe and returns the created <see cref="ImageAttachment"/>.
        /// </summary>
        /// <param name="image">The image to upload.</param>
        /// <param name="messageContainer">The <see cref="IMessageContainer"/> that the message is being sent to.</param>
        /// <param name="uploadProgress">A monitor that will receive progress updates for the image upload operation.</param>
        /// <returns>An <see cref="ImageAttachment"/> if uploaded successfully, null otherwise.</returns>
        public static async Task<ImageAttachment> CreateImageAttachment(byte[] image, IMessageContainer messageContainer, UploadProgress uploadProgress = null)
        {
            uploadProgress = uploadProgress ?? new UploadProgress();

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await messageContainer.Client.ExecuteRestRequestAsync(
                GroupMeImageApiUrl,
                image,
                "image/jpeg",
                cancellationTokenSource.Token,
                uploadProgress);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<ImageUploadResponse>(await restResponse.Content.ReadAsStringAsync());

                return new ImageAttachment()
                {
                    Url = result.Payload.Url,
                };
            }
            else
            {
                return null;
            }
        }

        private class ImageUploadResponse
        {
            [JsonProperty("payload")]
            public UploadPayload Payload { get; internal set; }

            public class UploadPayload
            {
                [JsonProperty("url")]
                public string Url { get; internal set; }

                [JsonProperty("picture_url")]
                public string PictureUrl { get; internal set; }
            }
        }
    }
}