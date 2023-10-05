namespace ThreadPool;

/// <summary>
/// Represents an asynchronous task with a result of type TResult.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Gets a value indicating whether the task has completed.
    /// </summary>
    bool IsCompleted { get; }
    
    /// <summary>
    /// Gets the result of the task.
    /// Blocks the thread waiting for the result to be calculated if it has not already occurred.
    /// </summary>
    /// <exception cref="AggregateException"> Error occurred during task calculation. </exception>
    TResult? Result { get; }

    /// <summary>
    /// Creates a new task that will be executed after the completion of the current task.
    /// </summary>
    /// <typeparam name="TNewResult">The type of the result produced by the new task.</typeparam>
    /// <param name="func">The delegate that will transform the result of the current task into the result of the new task.</param>
    /// <returns>A new task of type IMyTask </returns>
    /// <exception cref="InvalidOperationException"> New task can not be created. </exception>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func);
}
