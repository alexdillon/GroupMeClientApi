using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// <see cref="ContactsList"/> provides a list of <see cref="Contact"/>s along with additional status information.
    /// </summary>
    internal class ContactsList
    {
        /// <summary>
        /// Gets or sets the list of <see cref="Contact"/>s.
        /// </summary>
        [JsonProperty("response")]
        internal IList<Contact> Contacts { get; set; }

        /// <summary>
        /// Gets or sets the Metadata for the API Call.
        /// </summary>
        [JsonProperty("meta")]
        internal Meta Meta { get; set; }
    }
}
