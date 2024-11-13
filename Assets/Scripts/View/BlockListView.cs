using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockListView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _blockItemPrefab;
    [SerializeField] private GameObject _goEmpty;

    private void Awake()
    {
        RefreshUI();
        _backButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        UserData.Instance.OnDataChanged += OnDataChanged;
    }

    private void OnDestroy()
    {
        UserData.Instance.OnDataChanged -= OnDataChanged;
    }

    private void OnDataChanged()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        Utils.RefreshListItems(_scrollRect,_blockItemPrefab,UserData.Instance.BlockList.Count,((i, o) =>
        {
            var personId = UserData.Instance.BlockList[i];
            var config = ConfigLoader.Load<PersonConfigTable>().table[personId];
            o.transform.Find("Head").GetComponent<Image>().sprite = Utils.GetUserHead(config.head);
            o.transform.Find("TextName").GetComponent<Text>().text = config.name;
            o.transform.Find("Button_Remove").GetComponent<Button>().onClick.AddListener(() =>
            {
                UserData.Instance.RemoveBlock(config.id);
            });
        }));
        _goEmpty.SetActive(UserData.Instance.BlockList.Count == 0);
    }
        
    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/BlockListView"),Main.Instance.canvas.transform);
    }
}
