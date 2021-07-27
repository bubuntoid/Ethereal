using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace YOVPS.Core.Threading
{
    public class ThreadContext
    {
        public Guid Id { get; set; }
        public ICollection<ThreadWithId> Threads { get; set; }
    }
}