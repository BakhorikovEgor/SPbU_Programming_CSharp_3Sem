namespace Lazy;

public class ParallelLazy<T>
{
    private  Func<T>? _supplier;
    private  T? _value;
    
    private volatile LazyState _state;

    public ParallelLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _state = LazyState.NotReceived;
    }


    public T? Get()
    {
        // Reading _state updates _value and _supplier from memory!
        // This condition must be first!
        if (_state != LazyState.NotReceived)
        {
            return _value;
        }

        if (_supplier == null)
        {
            throw new InvalidOperationException("Null supplier is not allowed.");
        }
        
        lock (_supplier)
        {
            // Reading _state updates _value and _supplier from memory!
            // This condition must be first!
            if (_state == LazyState.ReceivedBySupplier)
            {
                return _value;
            }
            
            // Writing _state updates _value and _supplier in memory!
            // Writing _state should be the last statement!
            _value = _supplier();
            _supplier = null;
            
            _state = LazyState.ReceivedBySupplier;
            
            return _value;
        }
    }
}