using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using NLog;

namespace YOVPS.Core.Threading
{
    public static class ThreadQueue
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ICollection<ThreadContext> ThreadContexts { get; set; } = new List<ThreadContext>();

        public static void QueueTask(Guid contextId, Task task)
        {
            var context = ThreadContexts.FirstOrDefault(x => x.Id == contextId);
            if (context == null)
            {
                logger.Warn($"Registering new context with id {contextId}");
                context = new ThreadContext
                {
                    Id = contextId,
                    Threads = new List<ThreadWithId>()
                };

                ThreadContexts.Add(context);
            }

            var thread = new ThreadWithId
            {
                Id = Guid.NewGuid(),
            };
            thread.Thread = new Thread(async () =>
            {
                await task;
                context.Threads.Remove(thread);
            });
            context.Threads.Add(thread);
            
            thread.Thread.Start();
        }
        
        public static async Task WhenAll(Guid contextId, TimeSpan timeout)
        {
            var stopAt = DateTime.Now + timeout;
            
            while (true)
            {
                if (stopAt <= DateTime.Now)
                {
                    var thread = ThreadContexts.FirstOrDefault(x => x.Id == contextId);
                    if (thread != null)
                    {
                        ThreadContexts.Remove(thread);
                    }

                    logger.Warn($"Timeout exceeded, removing context {contextId} from queue");
                    throw new Exception("Something went wrong, try again"); // todo: use local exception;
                }
                
                var threadContext = ThreadContexts.FirstOrDefault(x => x.Id == contextId);
                if (threadContext == null)
                {
                    logger.Warn($"Thread context with id {contextId} does not exists in current thread contexts");
                    break;
                }
                
                if (threadContext.Threads.Any() == false)
                {
                    ThreadContexts.Remove(threadContext);
                    break;
                }

                await Task.Delay(10);
            }
        }
    }
}