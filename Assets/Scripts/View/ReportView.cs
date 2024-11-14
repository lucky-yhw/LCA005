using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportView : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _submitButton;

    private int _personId;
    
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
                UserData.Instance.Block(_personId);
                MsgView.Open("Submit Success");
                Destroy(gameObject);
            }
            else
            {
                MsgView.Open("Please Enter Your Suggestions!");
            }
        });
    }

    private void Init(int personId)
    {
        _personId = personId;
    }

    public static void Open(int personId)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/ReportView"),Main.Instance.canvas.transform);
        var script = go.GetComponent<ReportView>();
        script.Init(personId);
    }
}
