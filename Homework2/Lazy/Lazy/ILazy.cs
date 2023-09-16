namespace Lazy;

/// <summary>
/// Supply element of type T using lazy calculation.
///
/// Lazy calculation is a one-time calculation of the result, and its substitution in subsequent calls.
/// </summary>
/// <typeparam name="T"> Result element type. </typeparam>
public interface ILazy<out T>
{
    /// <summary>
    /// Lazy evaluation of the resulting element.
    /// </summary>
    /// <returns> T type element. </returns>
    public T? Get();
}