using System.Diagnostics;

namespace ThreadPoolTests;

public class MyTaskTests
{
    private static IEnumerable<TestCaseData> StandardSubmitFunctions
    {
        get
        {
            yield return new TestCaseData(() => 2 * 2, 4);
            yield return new TestCaseData(() => "Hello " + "World", "Hello World");
            yield return new TestCaseData(new Func<object?>(() => null), null);
        }
    }

    private static IEnumerable<TestCaseData> ThrowExceptionSubmitFunction
    {
        get
        {
            yield return new TestCaseData(new Func<object?>(() => throw new Exception()));
            yield return new TestCaseData(new Func<object?>(() => throw new DivideByZeroException()));
        }
    }


    [TestCaseSource(nameof(StandardSubmitFunctions))]
    public void SubmitFunction_ResultShouldBeCorrect<TResult>(Func<TResult> func, TResult correctResult)
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var task = pool.Submit(func);
        Assert.That(task.Result, Is.EqualTo(correctResult));
    }


    [TestCaseSource(nameof(ThrowExceptionSubmitFunction))]
    public void SubmitFunctionWithException_ResultThrowsException<TResult>(Func<TResult> func)
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var task = pool.Submit(func);
        Assert.Throws<AggregateException>(() =>
        {
            var temp = task.Result;
        });
    }
    
    
    [Test]
    public void ContinueWith_ShouldNotBlockMainThread()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        bool flag = false;
        
        var firstTask = pool.Submit(() =>
        {
            Thread.Sleep(10000);
            return 0;
        });

        var continuationTask = firstTask.ContinueWith(result =>
        {
            Volatile.Write(ref flag, true);
            return result + 1;
        });
        
        
        Assert.IsFalse(Volatile.Read(ref flag), "Main thread was blocked by ContinueWith.");
        
        pool.ShutDown();
    }
    
    
    [Test]
    public void Continuation_ShouldStartAfterParentTask()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        var flag = false;

        var parent = pool.Submit(() =>
        {
            Thread.Sleep(1000);
            return 0;
        });
        parent.ContinueWith(result =>
        {
            if (parent.IsCompleted)
            {
                Volatile.Write(ref flag, true);
            }

            return 1;
        });

        pool.ShutDown();

        Assert.That(Volatile.Read(ref flag), Is.True);
    }

    [Test]
    public void ContinuationAfterShutDown_ShouldThrowException()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var parent = pool.Submit(() => 1);
        pool.ShutDown();

        Assert.Throws<InvalidOperationException>(() => parent.ContinueWith(result => result * 2));
    }

    [Test]
    public void OldContinuationsAfterShutDown_ShouldBeDone()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);
        var counter = 0;

        for (var i = 0; i < Environment.ProcessorCount; ++i)
        {
            var task = pool.Submit(() => 0);

            task.ContinueWith((result) =>
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
    public void ContinuationIfParentNotDone_ShouldNotBlockThread()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var task = pool.Submit(() =>
        {
            Thread.Sleep(1000);
            return 0;
        });

        task.ContinueWith((result) => 0);

        Assert.That(task.IsCompleted, Is.False);
    }

    [Test]
    public void IsCompleteAfterResult_ShouldReturnTrue()
    {
        using var pool = new MyThreadPool(Environment.ProcessorCount);

        var task = pool.Submit(() =>
        {
            Thread.Sleep(1000);
            return 0;
        });
        var temp = task.Result;

        Assert.That(task.IsCompleted, Is.True);
    }
}