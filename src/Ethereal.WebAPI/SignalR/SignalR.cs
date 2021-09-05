using System.Collections.Generic;

namespace Ethereal.WebAPI.SignalR
{
    public static class SignalR
    {
        public static ICollection<WebSocketUserSession> Sessions { get; set; } =
            new List<WebSocketUserSession>();
    }
}