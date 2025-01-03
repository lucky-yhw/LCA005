using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallView : MonoBehaviour
{
    [SerializeField] private Image _headImage;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _textName;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
            MsgView.Open("Calling Canceled!");
        });
    }

    private void InitParams(PersonConfig personConfig)
    {
        _headImage.sprite = Utils.GetUserHead(personConfig.head);
        _textName.text = personConfig.name;
        StartCoroutine(CancelCall());
    }

    IEnumerator CancelCall()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
        MsgView.Open("Nobody Answers!");
    }

    public static void Open(PersonConfig personConfig)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/CallView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<CallView>();
        script.InitParams(personConfig);
    }
}