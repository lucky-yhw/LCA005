using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EULAView : MonoBehaviour
{
    [SerializeField] private Button _refuseButton;
    [SerializeField] private Button _acceptButton;

    private Action onClose;
    
    private void Awake()
    {
        _refuseButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        _acceptButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("EULA",1);
            PlayerPrefs.Save();
            Destroy(gameObject);
            onClose?.Invoke();
        });
    }

    public static void Open(Action onClose)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/EULAView"),
            Main.Instance.canvasTop.transform);
        go.GetComponent<EULAView>().onClose += onClose;
    }
}
