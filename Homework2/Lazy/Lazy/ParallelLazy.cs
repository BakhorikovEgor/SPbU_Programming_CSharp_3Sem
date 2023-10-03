namespace Lazy;

/// <summary>
/// Implementation of the ILazy interface for multi-threaded use.
/// </summary>
/// <typeparam name="T"> <inheritdoc cref="ILazy{T}"/></typeparam>
public class ParallelLazy<T> : ILazy<T>
{
    private T? _value;
    private Func<T>? _supplier;
    private Exception? _supplierException;
    
    private readonly object _locker = new();
    
    private volatile bool _valueReceived;

    /// <summary>
    /// Standard lazy object constructor.
    /// </summary>
    /// <param name="supplier"> Not nullable function - supplier of elements. </param>
    public ParallelLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _valueReceived = false;
    }


    /// <inheritdoc cref="ILazy{T}.Get"/>
    /// <exception cref="InvalidOperationException"> Supplier can not be null. </exception>
    /// <exception cref="Exception"> Exception got from supplier. </exception>
    public T? Get()
    {
        // Reading _valueReceived updates _value from memory!
        // This condition must be first!
        if (_valueReceived)
        {
            return _value;
        }

        if (_supplierException != null)
        {
            throw _supplierException;
        }

        if (_supplier == null)
        {
            throw new InvalidOperationException("Null supplier is not allowed.");
        }

        lock (_locker)
        {
            if (_valueReceived)
            {
                return _value;
            }
            
            if (_supplierException != null)
            {
                throw _supplierException;
            }

            try
            {
                _value = _supplier();
                
                // Writing _valueReceived updates _value!
                // Writing _state should be after writing _value, else _value can be null
                _valueReceived = true;
                
                // Should be last because _supplier null check after _state check!
                _supplier = null;
                
                return _value;
            }
            catch (Exception e)
            {
                _supplierException = e;
                throw;
            }
        }
    }
    
}