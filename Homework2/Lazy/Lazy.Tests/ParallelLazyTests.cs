namespace Lazy.Tests;

public class ParallelLazyTests
{
    private const int RestartCount = 1000;
    
    private static readonly int ThreadsCount = Environment.ProcessorCount;
    private static readonly Random Rand = new();
    
    private readonly ManualResetEvent _threadsHandler = new(false);

    
    [Test]
    public void GetBeforeNotNullValueIsSupplied_ShouldCorrectSynchronizeFirstThread_OthersReturnCorrectNotNullValue()
    {
        for (var i = 0; i < RestartCount; ++i)
        {
            var value = Rand.NextInt64();
            var lazy = new ParallelLazy<long>(() => value);
            var threads = new Thread[ThreadsCount];

            for (var j = 0; j < ThreadsCount; ++j)
            {
                threads[j] = new Thread(() =>
                {
                    _threadsHandler.WaitOne();
                    Assert.That(lazy.Get(), Is.EqualTo(value));
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            _threadsHandler.Set();
        }
    }
}