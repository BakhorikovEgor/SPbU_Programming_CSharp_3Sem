using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private ConcurrentQueue<Action> _tasks;
    private Thread[] _threads;

    public MyThreadPool(int count)
    {
        _tasks = new ConcurrentQueue<Action>();
        _threads = new Thread[count];
    }

    public IMyTask<TResult> QueueTask<TResult>(Func<TResult> func)
    {
        throw new NotImplementedException();
    }
}