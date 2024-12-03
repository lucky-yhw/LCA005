using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoadingView : MonoBehaviour
{
    private static LoadingView _instance;
    private Action _onFinished;
    
    private void Awake()
    {
        _instance = this;
    }


    private void OnDestroy()
    {
        _instance = null;
    }

    private void StopAutoClose()
    {
        StopAllCoroutines();
    }
    
    private void AutoClose()
    {
        StartCoroutine(CloseCor());
    }
    
    IEnumerator CloseCor()
    {
        var sec = Random.Range(0, 2f);
        yield return new WaitForSeconds(sec);
        CloseView();
        _onFinished?.Invoke();
    }

    private void CloseView()
    {
        Destroy(gameObject);
    }

    private static void LoadView()
    {
        if (!_instance)
        {
            var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/LoadingView"),
                Main.Instance.canvasTop.transform);
        }
        _instance.StopAutoClose();
    }
    
    public static void OpenAutoClose(Action onFinished)
    {
        LoadView();
        _instance._onFinished += onFinished;
        _instance.AutoClose();   
    }
    
    public static void Open(Action onFinished = null)
    {
        LoadView();
        _instance._onFinished += onFinished;
    }
    
    public static void Close()
    {
        if (_instance)
        {
            _instance.CloseView();   
        }
    }
}
