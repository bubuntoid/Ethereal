#nullable enable
using System;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Ethereal.WebAPI.SignalR
{
    public class ProcessingJobLoggerHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var queryParam = httpContext!.Request.Query.First(x => x.Key == "jobId").Value.First();
            var jobId = Guid.Parse(queryParam);
            
            var session = new WebSocketUserSession
            {
                ConnectionId = this.Context.ConnectionId,
                ProcessingJobId = jobId
            };
            SignalR.Sessions.Add(session);
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var session = SignalR.Sessions.FirstOrDefault(s => s.ConnectionId == Context.ConnectionId);
            if (session != null)
                SignalR.Sessions.Remove(session);

            return base.OnDisconnectedAsync(exception);
        }
    }
}