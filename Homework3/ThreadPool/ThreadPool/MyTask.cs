namespace ThreadPool;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly MyThreadPool _threadPool;
    private readonly ManualResetEvent _resultEvent;
    private readonly AutoResetEvent _computingEvent;
    private readonly List<Action> _delayedContinuations;
    
    private volatile bool _isCompleted;
    
    private Func<TResult>? _mainFunc;
    private Exception? _mainFuncException;
    private TResult? _result;
    
    
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

    
    public MyTask(MyThreadPool threadPool, Func<TResult> func)
    {
        _threadPool = threadPool;
        _mainFunc = func;

        _resultEvent = new ManualResetEvent(false);
        _computingEvent = new AutoResetEvent(true);
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
        if (_isCompleted)
        {
            return;
        }

        if (_mainFunc == null)
        {
            _mainFuncException = new Exception("Function can not be null. ");
            return;
        }
        
        _computingEvent.WaitOne();

        if (_isCompleted)
        {
            return;
        }

        try
        {
            _result = _mainFunc();
            _isCompleted = true;
            _mainFunc = null;

            _resultEvent.Set();
            SubmitDelayedContinuations();
        }
        catch (Exception e)
        {
            _mainFuncException = e;
        }
        finally
        {
            _isCompleted = true;
        }

        _computingEvent.Set();
    }


    private void SubmitDelayedContinuations()
        => _delayedContinuations.ForEach(c => _threadPool.Submit(c));
}