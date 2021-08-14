using System;
using System.Threading;

namespace Ethereal.Application.Threading
{
    public class ThreadWithId
    {
        public Guid Id { get; set; }
        public Thread Thread { get; set; }
    }
}