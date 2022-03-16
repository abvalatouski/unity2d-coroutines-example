using UnityEngine;
using UnityEngine.Events;

public class WaitForEvent : CustomYieldInstruction
{
    private UnityEvent @event;
    private bool wasFired;

    public WaitForEvent(UnityEvent @event)
    {
        this.@event = @event;
        @event.AddListener(SetWasFired);
        wasFired = false;
    }

    ~WaitForEvent()
    {
        @event.RemoveListener(SetWasFired);
    }

    public override bool keepWaiting
    {
        get
        {
            if (wasFired)
            {
                UnsetWasFired();
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void SetWasFired()
    {
        wasFired = true;
    }

    private void UnsetWasFired()
    {
        wasFired = false;
    }
}
