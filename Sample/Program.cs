using GroupMeClientApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new GroupMeClientApi.GroupMeClient("YOUR_API_TOKEN HERE");
            
            await client.GetGroupsAsync();
            await client.GetChatsAsync();
            ICollection<Contact> contacts = await client.GetContactsAsync();
            int x = contacts.Count;
            while (x > 0)
            {
                var sortedContacts = client.Contacts().OrderByDescending(c => c.CreatedAtIso8601Time);
                var earliestContact = sortedContacts.First();
                ICollection<Contact> _contacts = await client.GetContactsAsync(true, earliestContact.CreatedAtIso8601Time);
                x = _contacts.Count;
            }

            foreach (IMessageContainer messageContainer in Enumerable.Concat<IMessageContainer>(client.Groups(), client.Chats()))
            {
                Console.WriteLine($"{messageContainer.Name} - Updated {messageContainer.UpdatedAtTime.ToString()}");
            }

            int i = 1;
            foreach (Contact contact in client.Contacts())
            {
                Console.WriteLine($"{contact.Name} - Updated {contact.UpdatedAtTime.ToString()}      {i}");
                i++;
            }
        }
    }
}

