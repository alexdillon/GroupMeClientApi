using GroupMeClientApi.Models;
using System;
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

            foreach (IMessageContainer messageContainer in Enumerable.Concat<IMessageContainer>(client.Groups(), client.Chats()))
            {
                Console.WriteLine($"{messageContainer.Name} - Updated {messageContainer.UpdatedAtTime.ToString()}");
            }
        }
    }
}

