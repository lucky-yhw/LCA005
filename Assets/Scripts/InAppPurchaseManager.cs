using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class InAppPurchaseManager : IDetailedStoreListener
{
    private static InAppPurchaseManager _instance;
    public static InAppPurchaseManager Instance => _instance ??= new InAppPurchaseManager();

    private IStoreController _storeController;
    private IExtensionProvider _storeExtensionProvider;

    public delegate void OnPurchaseSuccessDelegate(Product product);

    public OnPurchaseSuccessDelegate onPurchaseSuccess;

    public delegate void OnPurchaseInitialized();

    public OnPurchaseInitialized onPurchaseInitialized;

    public bool IsInitialized()
    {
        return _storeController != null && _storeExtensionProvider != null;
    }

    public void Initialize(IEnumerable<string> productIdList)
    {
        if (IsInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var consumableProductId in productIdList)
        {
            builder.AddProduct(consumableProductId, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseBuy(string productId)
    {
        if (IsInitialized())
        {
            Product product = _storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                _storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("Product not found or not available for purchase");
            }
        }
        else
        {
            Debug.Log("Unity IAP not initialized");
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        onPurchaseSuccess?.Invoke(purchaseEvent.purchasedProduct);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _storeExtensionProvider = extensions;
        onPurchaseInitialized?.Invoke();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
    }

    public Product[] GetAllValidProducts()
    {
        if (!IsInitialized())
        {
            return new Product[] { };
        }

        return _storeController.products.all;
    }

    public string GetLocalizedPrizeString(string productId)
    {
        Product product = _storeController.products.WithID(productId);
        return product?.metadata.localizedPriceString;
    }
}