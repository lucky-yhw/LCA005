using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedBackView : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _submitButton;

    private void Awake()
    {
        _backButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        _submitButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(_inputField.text))
            {
                LoadingView.OpenAutoClose(() =>
                {
                    CommonTipsView.Open("Thank You For Feeding Back!", () =>
                    {
                        Destroy(gameObject);
                    });
                });
            }
            else
            {
                MsgView.Open("Please Enter Your Suggestions!");
            }
        });
    }
    
    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/FeedBackView"),Main.Instance.canvas.transform);
    }
}
