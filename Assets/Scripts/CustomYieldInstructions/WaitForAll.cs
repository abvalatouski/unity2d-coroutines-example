using System.Collections;

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
        get => stillRunning != 0;
    }

    private IEnumerator Run(IEnumerator coroutine)
    {
        yield return coroutine;
        stillRunning--;
    }
}
