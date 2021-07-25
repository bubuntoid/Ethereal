﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace YOVPS.Core.Extensions
{
    public static class ComputationExtensions
    {
        public static long ComputeElapsedTimeInMilliseconds(string actionName, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            Console.WriteLine($"{actionName} took {stopwatch.ElapsedMilliseconds} ms");
            return stopwatch.ElapsedMilliseconds;
        }
    }
}