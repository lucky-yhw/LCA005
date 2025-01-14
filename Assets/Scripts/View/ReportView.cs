using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportView : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _submitButton;

    private PersonConfig _person;

    private void Awake()
    {
        _backButton.onClick.AddListener(() => { Destroy(gameObject); });
        _submitButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(_inputField.text))
            {
                BlockUser();
            }
            else
            {
                MsgView.Open("Input Can't Be Empty");
            }
        });
    }

    private void BlockUser()
    {
        ServerData.Instance.Report(_person, () =>
        {
            CommonTipsView.Open(
                "We will thoroughly review the information you have reported, and we will address it within 72 hours. Thank you for your feedback and report.",
                () => { Destroy(gameObject); });
        }, () => { CommonTipsView.Open("Net error, please try again!", BlockUser); });
    }

    private void Init(PersonConfig person)
    {
        _person = person;
    }

    public static void Open(PersonConfig person)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/ReportView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<ReportView>();
        script.Init(person);
    }
}