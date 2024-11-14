using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonTipsView : MonoBehaviour
{
    [SerializeField] private Text _textContent;
    [SerializeField] private Button _buttonCancel;
    [SerializeField] private Button _buttonOk;
    
    private Action onOk;
    private Action onCancel;

    private void Awake()
    {
        _buttonCancel.onClick.AddListener(() =>
        {
            onCancel?.Invoke();
            Destroy(gameObject);
        });
        _buttonOk.onClick.AddListener(() =>
        {
            onOk?.Invoke();
            Destroy(gameObject);
        });
    }

    private void Init(string content,Action onOk, Action onCancel)
    {
        _textContent.text = content;
        this.onOk = onOk;
        this.onCancel = onCancel;
    }
    
    public static void Open(string content,Action onOk = null, Action onCancel = null)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/CommonTipsView"),
            Main.Instance.canvasPop.transform);
        go.transform.localPosition = Vector3.zero;
        var script = go.GetComponent<CommonTipsView>();
        script.Init(content,onOk,onCancel);
    }
}
