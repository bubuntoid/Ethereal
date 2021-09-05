using System;

namespace Ethereal.WebAPI.SignalR
{
    public class WebSocketUserSession
    {
        public string ConnectionId { get; set; }
        
        public Guid ProcessingJobId { get; set; }
    }
}