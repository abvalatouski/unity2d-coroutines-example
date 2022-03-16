using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;

public class WaitForTask<T> : CustomYieldInstruction
{
    private readonly Task monitoringTask;
    private readonly IntPtr resultDestination;
    private readonly IntPtr exceptionDestination;
    private readonly T cancellationFallback;
    private readonly T faultFallback;

    public WaitForTask(
        Task<T> task,
        out T result,
        out Exception exception,
        T cancellationFallback = default,
        T faultFallback = default)
        : this(task, Unout(out result), Unout(out exception), cancellationFallback, faultFallback)
    {
    }

    public WaitForTask(
        Task<T> task,
        out T result,
        T cancellationFallback = default,
        T faultFallback = default)
        : this(task, Unout(out result), default, cancellationFallback, faultFallback)
    {
    }

    private WaitForTask(
        Task<T> task,
        IntPtr resultDestination,
        IntPtr exceptionDestination,
        T cancellationFallback,
        T faultFallback)
    {
        monitoringTask = MonitorTask(task);
        this.resultDestination = resultDestination;
        this.exceptionDestination = exceptionDestination;
        this.cancellationFallback = cancellationFallback;
        this.faultFallback = faultFallback;
    }

    public override bool keepWaiting
    {
        get => !monitoringTask.IsCompleted;
    }

    private async Task MonitorTask(Task<T> task)
    {
        Unsafe.SkipInit(out T result);
        Unsafe.SkipInit(out Exception exception);
        try
        {
            result = await task;
            exception = default;
        }
        catch (TaskCanceledException cancellationException)
        {
            result = cancellationFallback;
            exception = cancellationException;
        }
        catch (Exception faultException)
        {
            result = faultFallback;
            exception = faultException;
        }
        finally
        {
            WriteTaskResult(result);
            WriteTaskException(exception);
        }
    }

    private unsafe void WriteTaskResult(T result)
    {
        Unsafe.Write<T>(resultDestination.ToPointer(), result);
    }

    private unsafe void WriteTaskException(Exception exception)
    {
        if (exceptionDestination == IntPtr.Zero)
        {
            return;
        }

        Unsafe.Write<Exception>(exceptionDestination.ToPointer(), exception);
    }

    private static unsafe IntPtr Unout<U>(out U parameter)
    {
        Unsafe.SkipInit(out parameter);
        return new IntPtr(Unsafe.AsPointer(ref parameter));
    }
}
