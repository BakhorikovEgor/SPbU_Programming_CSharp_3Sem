namespace Lazy;

public class SimpleLazy<T>
{
    private Func<T>? _supplier;
    private T? _value;
    private LazyState _state;
    
    public SimpleLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _state = LazyState.NotReceived;
    }

    
    public T? Get()
    {
        if (_state != LazyState.NotReceived)
        {
            return _value;
        }

        if (_supplier == null)
        {
            throw new InvalidOperationException("Null supplier is not allowed.");
        }

        _value = _supplier();
        _supplier = null;
        _state = LazyState.ReceivedBySupplier;

        return _value;
    }
}