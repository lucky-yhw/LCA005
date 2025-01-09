using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoutPopup : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _deleteAccountButton;

    private void Awake()
    {
        _closeButton.onClick.AddListener((() =>
        {
            Destroy(gameObject);
        }));
        _deleteAccountButton.onClick.AddListener((() =>
        {
            ServerData.Instance.Logoff(() =>
            {
                Application.Quit();
            });
        }));
    }


    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/LogoutPopup"),Main.Instance.canvas.transform);
    }
}
