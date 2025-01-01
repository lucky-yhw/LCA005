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

    private bool _blockListShouldRefresh = true;
    
    private void Awake()
    {
        RefreshUI();
        _backButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        UserData.Instance.OnBlockChanged += OnBlockChanged;
    }

    private void OnDestroy()
    {
        UserData.Instance.OnBlockChanged -= OnBlockChanged;
    }

    private void OnBlockChanged()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_blockListShouldRefresh)
        {
            return;
        }
        ServerData.Instance.GetBlockList((blockList) =>
        {
            _blockListShouldRefresh = false;
            Utils.RefreshListItems(_scrollRect,_blockItemPrefab,blockList.Count,((i, o) =>
            {
                var person = blockList[i];
                o.transform.Find("Head").GetComponent<Image>().sprite = Utils.GetUserHead(person.head);
                o.transform.Find("TextName").GetComponent<Text>().text = person.name;
                o.transform.Find("Button_Remove").GetComponent<Button>().onClick.AddListener(() =>
                {
                    // UserData.Instance.RemoveBlock(person.id);
                });
            }));
            _goEmpty.SetActive(blockList.Count == 0);
        }, () =>
        {
            CommonTipsView.Open("Net error, please try again!", RefreshUI);
        });
    }
        
    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/BlockListView"),Main.Instance.canvas.transform);
    }
}
