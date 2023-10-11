namespace Lazy.Tests;

public class LazyTests
{
    private static readonly Random Rand = new();
    
    private static int _counter;
    
    private static IEnumerable<TestCaseData> LazyImplementationsStandardSupplier
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<int>(() => Rand.Next()));
            yield return new TestCaseData(new ParallelLazy<int>(() => Rand.Next()));
        }
    }

    private static IEnumerable<TestCaseData> LazyImplementationsNullResultSupplier
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<object?>(() => null));
            yield return new TestCaseData(new ParallelLazy<object?>(() => null));
        }
    }

    private static IEnumerable<TestCaseData> LazyImplementationsExceptionThrowingSupplier
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<object?>(() => throw new Exception()));
            yield return new TestCaseData(new ParallelLazy<object?>(() => throw new Exception()));
        }
    }
    
    private static IEnumerable<TestCaseData> LazyImplementationChangingCounter
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<object?>(() =>
            {
                _counter++;
                return new object();
            }));
            yield return new TestCaseData(new ParallelLazy<object?>(() =>
            {
                _counter++;
                return new object();
            }));
        }
    }


    [TestCaseSource(nameof(LazyImplementationsStandardSupplier))]
    public void GetWithNotNullSupplier_ShouldReturnTheSameValueEveryTime(ILazy<int> lazy)
    {
        var firstResult = lazy.Get();
        var secondResult = lazy.Get();
        var thirdResult = lazy.Get();

        Assert.Multiple(() =>
        {
            Assert.That(firstResult, Is.EqualTo(secondResult));
            Assert.That(firstResult, Is.EqualTo(thirdResult));
        });
    }
    
    
    [TestCaseSource(nameof(LazyImplementationsNullResultSupplier))]
    public void GetWithNullSupplier_ShouldReturnNull(ILazy<object> lazy)
        => Assert.That(lazy.Get(), Is.Null);
    
    
    [TestCaseSource(nameof(LazyImplementationsExceptionThrowingSupplier))]
    public void GetWithExceptionThrowingSupplier_ShouldThrowException(ILazy<object> lazy)
        => Assert.Throws<Exception>(() => lazy.Get());

    
    [TestCaseSource(nameof(LazyImplementationChangingCounter))]
    public void GetTwice_ShouldUseSupplierOnce(ILazy<object> lazy)
    {
        _counter = 0;
        lazy.Get();
        lazy.Get();
        
        Assert.That(_counter, Is.EqualTo(1));
    }

}