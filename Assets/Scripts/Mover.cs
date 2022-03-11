using System.Collections;
using System.Threading;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour
{
    [SerializeField] private KeyPressListener keyPressListener;
    [SerializeField] private Image movee;
    [SerializeField] private float moveDuration;

    private WaitForEvent waitForKeyPress;
    private CancellationTokenSource cancellationTokenSource;
    private IProvider<Color32> colorProvider;

    private void Start()
    {
        waitForKeyPress = new WaitForEvent(keyPressListener.OnKeyPressed);
        cancellationTokenSource = new CancellationTokenSource();
        colorProvider = new ColorProvider();
        StartCoroutine(Movement());
    }
    
    private void OnDestroy()
    {
        cancellationTokenSource?.Dispose();
    }

    private IEnumerator Movement()
    {
        yield return RequestColor();

        while (true)
        {
            yield return waitForKeyPress;
            if (keyPressListener.LastKeyCode == KeyCode.Space)
            {
                StartCoroutine(RequestColor());
            }
        
            yield return movee.rectTransform
                .DOMove(
                    movee.rectTransform.rect.size * GetMovementDirection(),
                    moveDuration)
                .SetRelative()
                .WaitForCompletion();
        }
    }

    private IEnumerator RequestColor()
    {
        yield return new WaitForTask<Color32>(
            colorProvider.ProvideAsync(cancellationTokenSource.Token),
            out var color);
        Debug.LogFormat("#{0:X2}{1:X2}{2:X2} <color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
            color.r,
            color.g,
            color.b,
            "A color has arrived from the Internet!");
        movee.materialForRendering.color = color;
    }

    private Vector2 GetMovementDirection()
    {
        switch (keyPressListener.LastKeyCode)
        {
            case KeyCode.UpArrow:
            case KeyCode.W:
                return new Vector2(+0, +1);
            case KeyCode.RightArrow:
            case KeyCode.D:
                return new Vector2(+1, +0);
            case KeyCode.DownArrow:
            case KeyCode.S:
                return new Vector2(+0, -1);
            case KeyCode.LeftArrow:
            case KeyCode.A:
                return new Vector2(-1, +0);
            default:
                return new Vector2(+0, +0);
        }
    }
}
