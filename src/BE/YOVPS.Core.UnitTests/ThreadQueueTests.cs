using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using YOVPS.Core.Threading;

namespace YOVPS.Core.UnitTests
{
    [TestFixture]
    public class ThreadQueueTests
    {
        [Test]
        public void QueueTask_ContextCreated()
        {
            var id = Guid.NewGuid();
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddHours(1)));

            Assert.That(ThreadQueue.ThreadContexts.Any(c => c.Id == id));
        }

        [Test]
        public async Task WhenAll_ContextDoesNotExists_ErrorNotExpected()
        {
            await ThreadQueue.WhenAll(Guid.NewGuid(), TimeSpan.FromMinutes(1));
        }
        
        [Test]
        public async Task WhenAll_TaskCompleted_ContextRemoved()
        {
            var id = Guid.NewGuid();
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddSeconds(5)));

            await ThreadQueue.WhenAll(id, TimeSpan.FromMinutes(1));

            Assert.That(ThreadQueue.ThreadContexts.Any(c => c.Id == id), Is.False);
        }

        [Test]
        public void WhenAll_TaskTimeoutExceeded_ExceptionExpected_ContextRemoved()
        {
            var id = Guid.NewGuid();
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddHours(5)));

            Assert.CatchAsync(() => ThreadQueue.WhenAll(id, TimeSpan.FromSeconds(5)));

            Assert.That(ThreadQueue.ThreadContexts.Any(c => c.Id == id), Is.False);
        }
        
        [Test]
        public void WhenAll_MultipleTasks()
        {
            var id = Guid.NewGuid();
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddSeconds(1)));
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddSeconds(2)));
            ThreadQueue.QueueTask(id, BuildSelfDestructingTask(DateTime.Now.AddSeconds(3)));
            
            ThreadQueue.WhenAll(id, TimeSpan.FromMinutes(1));

            Assert.That(ThreadQueue.ThreadContexts.Any(c => c.Id == id));
        }
        
        private async Task BuildSelfDestructingTask(DateTime destructAt)
        {
            while (true)
            {
                if (DateTime.Now >= destructAt)
                {
                    break;
                }
                
                await Task.Delay(10);
            }
        }
    }
}