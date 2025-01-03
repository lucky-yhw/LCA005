using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Canvas _canvasPop;
    [SerializeField] private Canvas _canvasTop;
    
    public Canvas canvas => _canvas;
    public Canvas canvasPop => _canvasPop;
    public Canvas canvasTop => _canvasTop;
    
    public static Main Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DoStartUnityGameService();
    }

    private async void DoStartUnityGameService()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);
            StartCoroutine(DoStartPurchase());
        }
        catch (Exception exception)
        {
            // An error occurred during services initialization.
        }
    }

    public IEnumerator DoStartPurchase()
    {
        yield return new WaitForSeconds(5f);
        InAppPurchaseManager.Instance.Initialize(Const.ProductIdList);
        InAppPurchaseManager.Instance.onPurchaseSuccess += (product) =>
        {
            MsgView.Open("Purchase Success!");
            ServerData.Instance.GetGold(Const.InAppPurchaseId2Coins[product.definition.id]);
        };
    }
}
