using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseView : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }

    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/PurchaseView"),Main.Instance.canvas.transform);
    }
}
