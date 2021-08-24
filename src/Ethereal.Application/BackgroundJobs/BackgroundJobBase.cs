using System.Threading.Tasks;
using Hangfire;

namespace Ethereal.Application.BackgroundJobs
{
    [AutomaticRetry(Attempts = 0)]
    public abstract class BackgroundJobBase<T>
    {
        public abstract Task ExecuteAsync(T obj);

        public void Execute(T obj) => ExecuteAsync(obj)
            .GetAwaiter()
            .GetResult();
    }
}