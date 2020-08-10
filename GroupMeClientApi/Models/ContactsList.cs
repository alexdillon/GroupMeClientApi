using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// <see cref="ContactsList"/> provides a list of <see cref="Contact"/>s along with additional status information.
    /// </summary>
    public class ContactsList
    {
        /// <summary>
        /// Gets the list of <see cref="Contact"/>s.
        /// </summary>
        [JsonProperty("response")]
        public IList<Contact> Contacts { get; internal set; }

        /// <summary>
        /// Gets the Metadata for the API Call.
        /// </summary>
        [JsonProperty("meta")]
        public Meta Meta { get; internal set; }
    }
}
