namespace ThreadPoolTests;

public class MyThreadPoolTests
{
    private readonly ManualResetEvent _raceEvent = new ManualResetEvent(false);


    [Test]
    public void ShutDown_OldTasksShouldBedDone()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var counter = 0;
        for (var i = 0; i < Environment.ProcessorCount; ++i)
        {
            pool.Submit(() =>
            {
                Interlocked.Increment(ref counter);
                return 0;
            });
        }

        Thread.Sleep(1000);
        pool.ShutDown();

        Assert.That(counter, Is.EqualTo(Environment.ProcessorCount));
    }


    [Test]
    public void SubmitRace_AllTasksShouldBeDone()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var counter = 0;
        var threads = new Thread[Environment.ProcessorCount];
        for (var i = 0; i < Environment.ProcessorCount; ++i)
        {
            threads[i] = new Thread(() =>
            {
                _raceEvent.WaitOne();
                pool.Submit(() =>
                {
                    Interlocked.Increment(ref counter);
                    return 0;
                });
            });

            threads[i].Start();
        }

        _raceEvent.Set();

        Thread.Sleep(1000);
        pool.ShutDown();

        Assert.That(counter, Is.EqualTo(Environment.ProcessorCount));
    }


    [Test]
    public void NumberOfWorkingThreads_ShouldEqualToStarted()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        Assert.That(pool.GetNumberOfAbleThreads(), Is.EqualTo(Environment.ProcessorCount));
    }


    [Test]
    public void NoTasks_ThreadsShouldWait()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        Assert.That(pool.GetNumberOfWorkingThreads(), Is.EqualTo(0));
    }


    [Test]
    public void ShutDown_ThreadShouldBeDone()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        pool.ShutDown();

        Assert.That(pool.GetNumberOfAbleThreads(), Is.EqualTo(0));
    }

    [Test]
    public void ShutDown_SubmitThrowsException()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        pool.ShutDown();

        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 0));
    }
}