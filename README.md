# GroupMeClientApi - C# Client for the GroupMe API

### License: Apache License 2.0
### Features

* Groups supported
* Chats (Direct Messages) support
* Send and receive messages
* Push Notification support
* Support for the GroupMe Image API
* Support for GroupMe Attachments (Images, Linked Images, Mentions, Locations, Emojis, and Videos)
* OAuth authentication support
* Cross Platform, targetting .NET Standard 2.0.

Find [GroupMeClientApi](https://www.nuget.org/packages/GroupMeClientApi/) on NuGet.

### Sample
```csharp
var client = new GroupMeClientApi.GroupMeClient("YOUR_API_TOKEN HERE");

await client.GetGroupsAsync();
await client.GetChatsAsync();

foreach (IMessageContainer messageContainer in Enumerable.Concat<IMessageContainer>(client.Groups(), client.Chats()))
{
    Console.WriteLine($"{messageContainer.Name} - Updated {messageContainer.UpdatedAtTime.ToString()}");
}
