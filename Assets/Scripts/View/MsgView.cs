using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MsgView : MonoBehaviour
{
    [SerializeField] private Text _text;

    private void ShowText(string text)
    {
        _text.text = text;
        (transform as RectTransform).ForceRebuildImmediate(true);
        transform.GetComponent<CanvasGroup>().DOFade(0, 1f).SetDelay(1f);
        transform.DOLocalMoveY(600, 2f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
    public static void Open(string text)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/MsgView"),
            Main.Instance.canvasPop.transform);
        go.transform.localPosition = Vector3.zero;
        var script = go.GetComponent<MsgView>();
        script.ShowText(text);
    }
}