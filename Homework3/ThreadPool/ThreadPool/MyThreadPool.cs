namespace ThreadPool;

/// <summary>
/// Represents a custom thread pool for managing and executing asynchronous tasks.
/// </summary>
public class MyThreadPool : IDisposable
{
    private readonly Queue<Action> _tasks;
    private readonly MyThreadPoolThread[] _threads;
    private readonly AutoResetEvent _accessToTasksEvent;
    private readonly AutoResetEvent _threadWakeUpBeforeCancelEvent;
    private readonly ManualResetEvent _threadWakeUpAfterCancelEvent;
    private readonly WaitHandle[] _threadWakeUpHandlers;
    private readonly CancellationTokenSource _tokenSource;


    /// <summary>
    /// Initializes a new instance of the MyThreadPool class with the specified number of worker threads.
    /// </summary>
    /// <param name="count">The number of worker threads to create in the thread pool.</param>
    public MyThreadPool(int count)
    {
        _tasks = new Queue<Action>();
        _threads = new MyThreadPoolThread[count];
        _tokenSource = new CancellationTokenSource();
        _accessToTasksEvent = new AutoResetEvent(true);
        _threadWakeUpBeforeCancelEvent = new AutoResetEvent(false);
        _threadWakeUpAfterCancelEvent = new ManualResetEvent(false);
        _threadWakeUpHandlers = new WaitHandle[]
        {
            _threadWakeUpBeforeCancelEvent,
            _threadWakeUpAfterCancelEvent
        };

        InitThreads();
    }


    // <summary>
    /// Submits a new asynchronous task to the thread pool and returns an IMyTask<TResult> representing the task.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="func">The function to be executed asynchronously.</param>
    /// <returns>An IMyTask<TResult> representing the submitted task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (_tokenSource.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException("ThreadPool was shut down");
        }

        var task = new MyTask<TResult>(this, func);
        Submit(task.Compute);

        return task;
    }

    /// <summary>
    /// Shuts down the thread pool, preventing the submission of new tasks.
    /// Already submitted tasks and their continuations will be completed.
    /// </summary>
    public void ShutDown()
    {
        if (_tokenSource.Token.IsCancellationRequested)
        {
            return;
        }

        _tokenSource.Cancel();
        _threadWakeUpAfterCancelEvent.Set();

        while (true)
        {
            if (_threads.All(thread => thread.IsDone))
            {
                break;
            }
        }
    }

    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _accessToTasksEvent.Dispose();
        _threadWakeUpBeforeCancelEvent.Dispose();
        _threadWakeUpAfterCancelEvent.Dispose();
        _tokenSource.Dispose();
        ShutDown();
    }


    private void Submit(Action func)
    {
        _accessToTasksEvent.WaitOne();
        _tasks.Enqueue(func);
        _threadWakeUpBeforeCancelEvent.Set();
        _accessToTasksEvent.Set();
    }


    private void InitThreads()
    {
        for (var i = 0; i < _threads.Length; ++i)
        {
            _threads[i] = new MyThreadPoolThread(this);
        }
    }
    

    private class MyThreadPoolThread
    {
        private readonly MyThreadPool _threadPool;
        
        internal bool IsDone { get; private set; }


        internal MyThreadPoolThread(MyThreadPool threadPool)
        {
            _threadPool = threadPool;

            IsDone = false;
            
            var thread = new Thread(Start);
            thread.Start();
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
                    WaitHandle.WaitAny(_threadPool._threadWakeUpHandlers);
                }
            }

            IsDone = true;
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly MyThreadPool _threadPool;
        private readonly ManualResetEvent _resultEvent;
        private readonly List<Action> _delayedContinuations;

        private Func<TResult>? _mainFunc;
        private Exception? _mainFuncException;
        private TResult? _result;
        private bool _isCompleted;


        public bool IsCompleted => _isCompleted;


        public TResult? Result
        {
            get
            {
                _resultEvent.WaitOne();
                if (_mainFuncException != null)
                {
                    throw new AggregateException(_mainFuncException);
                }

                return _result;
            }
        }


        internal MyTask(MyThreadPool threadPool, Func<TResult> func)
        {
            _threadPool = threadPool;
            _mainFunc = func;

            _resultEvent = new ManualResetEvent(false);
            _delayedContinuations = new List<Action>();

            _isCompleted = false;
        }


        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func)
        {
            if (_isCompleted)
            {
                return _threadPool.Submit(() => func(Result));
            }

            var delayedContinuation = new MyTask<TNewResult>(_threadPool, () => func(Result));
            _delayedContinuations.Add(() => delayedContinuation.Compute());

            return delayedContinuation;
        }


        internal void Compute()
        {
            if (_mainFunc == null)
            {
                _mainFuncException = new Exception("Function can not be null. ");
                return;
            }

            try
            {
                _result = _mainFunc();
            }
            catch (AggregateException e)
            {
                _mainFuncException = new Exception("Parent task thrown exception", e);
            }
            catch (Exception e)
            {
                _mainFuncException = e;
            }
            finally
            {
                _mainFunc = null;
                _isCompleted = true;

                _resultEvent.Set();
                SubmitDelayedContinuations();
            }
        }


        private void SubmitDelayedContinuations()
            => _delayedContinuations.ForEach(continuation => _threadPool.Submit(continuation));
    }
}