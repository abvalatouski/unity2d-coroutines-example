using System.Collections;
using System.Threading;

using UnityEngine;

public class WaitForAll : CustomYieldInstruction
{
    private int stillRunning;

    public WaitForAll(MonoBehaviour script, params IEnumerator[] coroutines)
    {
        stillRunning = coroutines.Length;
        for (var i = 0; i < coroutines.Length; i++)
        {
            script.StartCoroutine(Run(coroutines[i]));
        }
    }

    public override bool keepWaiting
    {
        get
        {
            int stillRunning = Interlocked.CompareExchange(ref this.stillRunning, 0, 0);
            return stillRunning != 0;
        }
    }

    private IEnumerator Run(IEnumerator coroutine)
    {
        yield return coroutine;
        Interlocked.Decrement(ref stillRunning);
    }
}
