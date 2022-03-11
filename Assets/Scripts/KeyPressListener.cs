using UnityEngine;
using UnityEngine.Events;

public class KeyPressListener : MonoBehaviour
{
    [field: SerializeField] public UnityEvent OnKeyPressed { get; private set; }

    private bool isKeyPressed;

    public KeyCode LastKeyCode { get; private set; }

    private void Start()
    {
        isKeyPressed = false;
    }

    private void OnGUI()
    {
        Event @event = Event.current;
        switch (@event.type)
        {
            case EventType.KeyDown when !isKeyPressed:
                isKeyPressed = true;
                LastKeyCode = @event.keyCode;
                OnKeyPressed.Invoke();
                break;
            case EventType.KeyUp:
                isKeyPressed = false;
                break;
        }
    }
}
