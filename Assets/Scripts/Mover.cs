using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour
{
    [SerializeField] private KeyPressListener keyPressListener;
    [SerializeField] private Image movee;
    [SerializeField] private float moveDuration;
    [SerializeField] private float scaleDuration;

    private CancellationTokenSource cancellationTokenSource;
    private IProvider<Color32> colorProvider;
    private WaitForEvent waitForKeyPress;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        colorProvider = new ColrOrgProvider();
    }

    private void Start()
    {
        waitForKeyPress = new WaitForEvent(keyPressListener.OnKeyPressed);
        StartCoroutine(Movement());
    }
    
    private void OnDestroy()
    {
        movee.materialForRendering.color = Color.white;
        cancellationTokenSource?.Dispose();
    }

    private IEnumerator Movement()
    {
        yield return new WaitForAll(this,
            RequestColor(CancellationToken.None),
            YoyoishScale());

        while (true)
        {
            yield return waitForKeyPress;
            if (keyPressListener.LastKeyCode == KeyCode.Space)
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                GCHandle.Alloc(
                    StartCoroutine(RequestColor(cancellationTokenSource.Token)),
                    GCHandleType.Pinned);
            }
            else if (keyPressListener.LastKeyCode == KeyCode.Backspace)
            {
                cancellationTokenSource.Cancel();
            }

            yield return Move();    
        }
    }

    private IEnumerator RequestColor(CancellationToken cancellationToken)
    {
        yield return new WaitForTask<Color32>(
            colorProvider.ProvideAsync(cancellationToken),
            out var color,
            out var exception);
        if (exception is null)
        {
            Debug.LogFormat("#{0:X2}{1:X2}{2:X2} <color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
                color.r,
                color.g,
                color.b,
                "A color has arrived from the Internet!");
            movee.materialForRendering.color = color;
        }
        else
        {
            Debug.Log(exception);
        }
    }

    private IEnumerator YoyoishScale()
    {
        yield return movee.rectTransform
            .DOScale(2, scaleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)
            .WaitForCompletion();
    }

    private IEnumerator Move()
    {
        yield return movee.rectTransform
            .DOMove(
                movee.rectTransform.rect.size * GetMovementDirection(),
                moveDuration)
            .SetRelative()
            .WaitForCompletion();
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
