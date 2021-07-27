using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using NLog;

namespace YOVPS.Core
{
    public static class ThreadQueue
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        public static IDictionary<Guid, List<Thread>> ThreadContexts { get; set; } =
            new Dictionary<Guid, List<Thread>>();

        public static void QueueAction(Guid contextId, Action action)
        {
            if (ThreadContexts.ContainsKey(contextId) == false)
            {
                ThreadContexts.Add(contextId, new List<Thread>());
            }

            var context = ThreadContexts.First(x => x.Key == contextId);
            var threadId = Guid.NewGuid().ToString();
            var thread = new Thread(() =>
            {
                // Perform action
                action.Invoke();

                // Delete from context queue
                var _ = context.Value.First(x => x.Name == threadId);
                context.Value.Remove(_);
            }) {Name = threadId};
            thread.Start();
            
            context.Value.Add(thread);
        }
        
        public static async Task WhenAll(Guid contextId, TimeSpan timeout)
        {
            var stopAt = DateTime.Now + timeout;
            
            while (true)
            {
                if (stopAt <= DateTime.Now)
                {
                    ThreadContexts.Remove(contextId);
                    logger.Warn($"Timeout exceeded, removing context {contextId} from queue");
                    throw new Exception("Something went wrong, try again"); // todo: use local exception;
                    break;
                }
                
                if (ThreadContexts.ContainsKey(contextId) == false)
                    break;

                var threadContext = ThreadContexts.FirstOrDefault(x => x.Key == contextId);
                if (threadContext.Value.Any() == false)
                {
                    ThreadContexts.Remove(threadContext);
                    break;
                }

                await Task.Delay(10);
            }
        }
    }
}