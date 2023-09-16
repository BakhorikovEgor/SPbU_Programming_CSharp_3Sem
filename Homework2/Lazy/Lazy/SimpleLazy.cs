namespace Lazy;


/// <summary>
/// Implementation of the ILazy interface for single-threaded use.
/// </summary>
/// <typeparam name="T"> <inheritdoc cref="ILazy{T}"/></typeparam>
public class SimpleLazy<T> : ILazy<T>
{
    private Func<T>? _supplier;
    private T? _value;
    private LazyState _state;

    /// <summary>
    /// Standard lazy object constructor.
    /// </summary>
    /// <param name="supplier"> Not nullable function - supplier of elements. </param>
    public SimpleLazy(Func<T> supplier)
    {
        _supplier = supplier;
        _state = LazyState.NotReceived;
    }

    /// <inheritdoc cref="ILazy{T}.Get"/>
    /// <exception cref="InvalidOperationException"> Supplier can not be null. </exception>
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