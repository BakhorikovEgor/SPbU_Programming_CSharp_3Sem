namespace MyThreadPool;

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
    
    
    private void Compute()
    {
        if (_isCompleted || _mainFuncException == null)
        {
            return;
        }

        if (_mainFunc == null)
        {
            _mainFuncException = new Exception("Function can not be null. ");
            return;
        }
        
        _computingEvent.WaitOne();

        if (_isCompleted || _mainFuncException == null)
        {
            return;
        }

        try
        {
            _result = _mainFunc();
            _mainFunc = null;
            _isCompleted = true;

            _resultEvent.Set();
            SubmitDelayedContinuations();
        }
        catch (Exception e)
        {
            _mainFuncException = e;
        }

        _computingEvent.Set();
    }


    private void SubmitDelayedContinuations()
        => _delayedContinuations.ForEach(c => _threadPool.SubmitContinuation(c));
}