using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Selectable))]
public class ScalableSelectable : UIBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float downScale = 0.9f;
    public float upScale = 1.1f;
    public float downDuration = 0.2f;
    public float upDuration = 0.1f;
    public float resetDuration = 0.1f;

    private Selectable _selectable;

    public Selectable Selectable
    {
        get
        {
            if (!_selectable)
            {
                _selectable = GetComponent<Selectable>();
            }

            return _selectable;
        }
    }

    public bool Interactable => Selectable.enabled && Selectable.interactable;

    private Tween _scaleTween;
    private bool _isDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactable)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(Vector3.one * downScale, downDuration);
            _isDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDown)
        {
            _scaleTween?.Kill();
            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one * upScale, upDuration));
            seq.Append(transform.DOScale(Vector3.one, resetDuration));
            _scaleTween = seq;
            _isDown = false;
        }
    }

    protected override void OnEnable()
    {
        _scaleTween?.Kill();
        _isDown = false;
        transform.localScale = Vector3.one;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _scaleTween?.Kill();
    }
}