using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;

public unsafe class WaitForTask<T> : CustomYieldInstruction
{
    private readonly Task<T> task;
    private readonly void* result;
    private readonly T cancellationFallback;
    private readonly T faultFallback;

    public WaitForTask(
        Task<T> task,
        out T result,
        T cancellationFallback = default,
        T faultFallback = default)
    {
        Unsafe.SkipInit(out result);
        this.task = task;
        this.result = Unsafe.AsPointer(ref result);
        this.cancellationFallback = cancellationFallback;
        this.faultFallback = faultFallback;
    }

    public override bool keepWaiting
    {
        get
        {
            ref T result = ref Unsafe.AsRef<T>(this.result);
            if (task.IsCompleted)
            {
                result = task.Result;
                return false;
            }
            else if (task.IsCanceled)
            {
                result = cancellationFallback;
                return false;
            }
            else if (task.IsFaulted)
            {
                result = faultFallback;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
