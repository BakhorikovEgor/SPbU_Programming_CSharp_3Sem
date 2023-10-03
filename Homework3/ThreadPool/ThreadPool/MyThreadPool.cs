namespace ThreadPool;

public class MyThreadPool : IDisposable
{
    private readonly Queue<Action> _tasks;
    private readonly AutoResetEvent _accessToTasksEvent;
    private readonly AutoResetEvent _threadWakeUpEvent;
    private readonly MyThreadPoolThread[] _threads;
    private readonly CancellationTokenSource _tokenSource;


    public MyThreadPool(int count)
    {
        _tasks = new Queue<Action>();
        _accessToTasksEvent = new AutoResetEvent(true);
        _threadWakeUpEvent = new AutoResetEvent(false);
        _tokenSource = new CancellationTokenSource();
        _threads = new MyThreadPoolThread[count];

        for (var i = 0; i < count; ++i)
        {
            _threads[i] = new MyThreadPoolThread(this);
        }
    }


    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (_tokenSource.Token.IsCancellationRequested)
        {
            throw new Exception();
        }
        
        var task = new MyTask<TResult>(this, func);
        Submit(task.Compute);

        return task;
    }


    public void ShutDown()
    {
        if (_tokenSource.Token.IsCancellationRequested) return;
        
        _tokenSource.Cancel();
        for (var i = 0; i < _threads.Length; ++i)
        {
            _threadWakeUpEvent.Set();
        }

    }

    internal void Submit(Action func)
    {
        _accessToTasksEvent.WaitOne();
        _tasks.Enqueue(func);
        _threadWakeUpEvent.Set();
        _accessToTasksEvent.Set();
    }


    private class MyThreadPoolThread
    {
        private readonly MyThreadPool _threadPool;
        private readonly Thread _thread;

        public bool IsWorking { get; private set; }


        public MyThreadPoolThread(MyThreadPool threadPool)
        {
            _threadPool = threadPool;
            IsWorking = true;
            _thread = new Thread(Start);
            _thread.Start();
        }

        private void Start()
        {
            while (true)
            {
                _threadPool._accessToTasksEvent.WaitOne();
                if (_threadPool._tasks.TryDequeue(out var task))
                {
                    _threadPool._accessToTasksEvent.Set();
                    task();
                }
                else if (_threadPool._tokenSource.Token.IsCancellationRequested)
                {
                    _threadPool._accessToTasksEvent.Set();
                    break;
                }
                else
                {
                    _threadPool._accessToTasksEvent.Set();
                    IsWorking = false;
                    _threadPool._threadWakeUpEvent.WaitOne();
                    IsWorking = true;
                }
            }
        }
    }

    public void Dispose()
    {
        _accessToTasksEvent.Dispose();
        _threadWakeUpEvent.Dispose();
        _tokenSource.Dispose();
    }
}