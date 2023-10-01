namespace MyThreadPool;

//BLOCKINGCOLLECITON
public class MyThreadPool
{
    private readonly Queue<Action> _tasks;
    private readonly Thread[] _threads;
    private readonly AutoResetEvent _gate;
    
    private readonly CancellationToken _token;
    
    public MyThreadPool(int count, CancellationToken token)
    {
        _tasks = new Queue<Action>();
        _threads = new Thread[count];
        _gate = new AutoResetEvent(false);
        _token = token;

        for (var i = 0; i < count; ++i)
        {
            _threads[i] = new Thread(() =>
            {
                while (!_token.IsCancellationRequested)
                {
                    if (_tasks.Count > 0)
                    {
                        var task = _tasks!.Dequeue();
                        task();
                    }
                    else
                    {
                        _gate.WaitOne();
                    }
                }
            });
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        throw new NotImplementedException();
    }

    internal void SubmitContinuation(Action continuation)
    {
        throw new NotImplementedException();
    }
    
}