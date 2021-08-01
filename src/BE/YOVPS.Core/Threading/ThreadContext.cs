using System;
using System.Collections.Generic;

namespace YOVPS.Core.Threading
{
    public class ThreadContext
    {
        public Guid Id { get; set; }
        public ICollection<ThreadWithId> Threads { get; set; }
    }
}