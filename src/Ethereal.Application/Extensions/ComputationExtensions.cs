using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;

namespace Ethereal.Application.Extensions
{
    public static class ComputationExtensions
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        public static long ComputeElapsedTimeInMilliseconds(string actionName, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            logger.Info($"{actionName} took {stopwatch.ElapsedMilliseconds} ms");
            return stopwatch.ElapsedMilliseconds;
        }
        
        public static async Task ComputeElapsedTimeInMillisecondsAsync(string actionName, Task action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await action;
            stopwatch.Stop();
            logger.Info($"{actionName} took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}