using System;
using System.Collections.Generic;

namespace Ethereal.Application.Threading
{
    public class ThreadContext
    {
        public Guid Id { get; set; }
        public ICollection<ThreadWithId> Threads { get; set; }
    }
}