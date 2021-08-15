using System.Threading.Tasks;

namespace Ethereal.Application.BackgroundJobs
{
    public abstract class BackgroundJobBase<T>
    {
        public abstract Task ExecuteAsync(T obj);

        public void Execute(T obj) => ExecuteAsync(obj)
            .GetAwaiter()
            .GetResult();
    }
}