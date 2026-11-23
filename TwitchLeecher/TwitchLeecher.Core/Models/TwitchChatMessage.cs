using System;

namespace TwitchLeecher.Core.Models
{
    // One VOD chat message; Offset is from the start of the VOD (#71)
    public record TwitchChatMessage(TimeSpan Offset, string Name, string Text);
}
