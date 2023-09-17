﻿namespace Lazy;

/// <summary>
/// Implementation of the ILazy interface for single-threaded use.
/// </summary>
/// <typeparam name="T"> <inheritdoc cref="ILazy{T}"/></typeparam>
public class SimpleLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private T? _value;
    private Exception? _supplierException;
    private bool _valueReceived;

    /// <summary>
    /// Standard lazy object constructor.
    /// </summary>
    /// <param name="supplier"> Not nullable function - supplier of elements. </param>
    public SimpleLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _valueReceived = false;
    }

    /// <inheritdoc cref="ILazy{T}.Get"/>
    /// <exception cref="InvalidOperationException"> Supplier can not be null. </exception>
    /// <exception cref="Exception"> Exception got from supplier. </exception>
    public T? Get()
    {
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

        try
        {
            _value = _supplier();
            _valueReceived = true;
            _supplier = null;
        }
        catch (Exception e)
        {
            _supplierException = e;
            throw _supplierException;
        }

        return _value;
    }
}