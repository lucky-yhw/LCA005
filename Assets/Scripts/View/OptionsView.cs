using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    
    [SerializeField] private Button _buttonBlock;
    [SerializeField] private Button _buttonReport;
    [SerializeField] private Button _buttonClose;


    private PersonConfig _person;
    // Start is called before the first frame update
    void Start()
    {
        _buttonBlock.onClick.AddListener(() =>
        {
            BlockUser();
        });
        _buttonReport.onClick.AddListener(() =>
        {
            ReportView.Open(_person);
            Destroy(gameObject);
        });
        _buttonClose.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }

    private void BlockUser()
    {
        ServerData.Instance.Block(_person,1, () =>
        {
            CommonTipsView.Open("Block Success!", () =>
            {
                Destroy(gameObject);
            });
        }, () =>
        {
            CommonTipsView.Open("Net error, please try again!", BlockUser);
        });
    }
    
    private void Init(PersonConfig person)
    {
        _person = person;
    }
    
    public static void Open(PersonConfig person)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/OptionsView"),
            Main.Instance.canvasPop.transform);
        var script = go.GetComponent<OptionsView>();
        script.Init(person);
    }
}
