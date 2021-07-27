using System;
using System.Threading;

namespace YOVPS.Core.Threading
{
    public class ThreadWithId
    {
        public Guid Id { get; set; }
        public Thread Thread { get; set; }
    }
}