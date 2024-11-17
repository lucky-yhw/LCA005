using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseView : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _textGold;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _purchaseItemPrefab;
    private void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        if (InAppPurchaseManager.Instance.IsInitialized())
        {
            InitPurchaseItems();
        }
        else
        {
            InAppPurchaseManager.Instance.onPurchaseInitialized += OnPurchaseInitialized;
        }

        _textGold.text = Utils.FormatGold(UserData.Instance.Gold);
        UserData.Instance.OnDataChanged += OnDataChanged;
    }

    private void OnDataChanged()
    {
        _textGold.text = Utils.FormatGold(UserData.Instance.Gold);
    }

    private void OnDestroy()
    {
        InAppPurchaseManager.Instance.onPurchaseInitialized += OnPurchaseInitialized;
    }


    private  void InitPurchaseItems()
    {
        var products = InAppPurchaseManager.Instance.GetAllValidProducts();
        Utils.RefreshListItems(_scrollRect,_purchaseItemPrefab,products.Length,((i, o) =>
        {
            var productId = products[i].definition.id;
            o.transform.Find("Gold/Text").GetComponent<Text>().text =
                Utils.FormatGold(Const.InAppPurchaseId2Coins[productId]);
            o.transform.Find("Btn/Text").GetComponent<Text>().text =
                InAppPurchaseManager.Instance.GetLocalizedPrizeString(productId);
            o.transform.Find("icon").GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Textures/PurchaseView/coin" + (i + 1));
            o.GetComponent<Button>().onClick.AddListener(() =>
            {
                InAppPurchaseManager.Instance.PurchaseBuy(productId);
            });
        }));
    }
    
    private void OnPurchaseInitialized()
    {
        InAppPurchaseManager.Instance.onPurchaseInitialized -= OnPurchaseInitialized;
        InitPurchaseItems();
    }
    
    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/PurchaseView"),Main.Instance.canvas.transform);
    }
}
