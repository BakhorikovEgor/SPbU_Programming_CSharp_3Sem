namespace Lazy;

/// <summary>
/// Implementation of the ILazy interface for multi-threaded use.
/// </summary>
/// <typeparam name="T"> <inheritdoc cref="ILazy{T}"/></typeparam>
public class ParallelLazy<T> : ILazy<T>
{
    private T? _value;
    private  Func<T>? _supplier;
    private volatile LazyState _state;

    /// <summary>
    /// Standard lazy object constructor.
    /// </summary>
    /// <param name="supplier"> Not nullable function - supplier of elements. </param>
    public ParallelLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _state = LazyState.NotReceived;
    }


    /// <inheritdoc cref="ILazy{T}.Get"/>
    /// <exception cref="InvalidOperationException"> Supplier can not be null. </exception>
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
            // Reading _state updates _value from memory!
            // This condition must be first!
            if (_state == LazyState.ReceivedBySupplier)
            {
                return _value;
            }

            _value = _supplier();
            
            // Writing _state updates _value!
            // Writing _state should be after writing _value!
            // Else _value can be null
            _state = LazyState.ReceivedBySupplier;
            
            // Should be last because _supplier null check after _state check!
            _supplier = null;
        }

        return _value;
    }
}