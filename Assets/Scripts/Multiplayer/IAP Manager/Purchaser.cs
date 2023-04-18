using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using static UnityEngine.Debug;
using static Client;
using BestHTTP;
using Newtonsoft.Json;
using System.Text;
using AppsFlyerSDK;

[Serializable]
class URLDto
{
    public string Message { get; set; }
    public string Value { get; set; }
}

public class Purchaser : MonoBehaviour, IStoreListener
{
    [SerializeField] private SampleWebView webView;
    public static Purchaser Instance { get; private set; }
    public event Action<int> OnSpinPurchased;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private bool IsInitialized => m_StoreController != null && m_StoreExtensionProvider != null;
    private Dictionary<string, SpinType> spinProductsIdDictionary = new Dictionary<string, SpinType>()
    {
        {"com.mygame.pokerhand.spindollars0.99", SpinType.WhitePower},
        {"com.mygame.pokerhand.spindollars10.99", SpinType.BluePower},
        {"com.mygame.pokerhand.spindollars25.99", SpinType.OrangePower},
        {"com.mygame.pokerhand.spindollars79.99", SpinType.PurplePower},
    };
    private Dictionary<string, int> chipsProductsDictionary = new Dictionary<string, int>()
    {
        {"com.mygame.pokerhand.chipsdollars0.99", 30_000},
        {"com.mygame.pokerhand.chipsdollars3.99", 105_000},
        {"com.mygame.pokerhand.chipsdollars5.99", 215_000},
        {"com.mygame.pokerhand.chipsdollars9.99", 450_000},
        {"com.mygame.pokerhand.chipsdollars17.99", 1_000_000},
        {"com.mygame.pokerhand.chipsdollars25.99", 1_750_000},
        {"com.mygame.pokerhand.chipsdollars49.99", 5_000_000},
        {"com.mygame.pokerhand.chipsdollars99.99", 16_500_000}
    };
    private Dictionary<ItemForPurchaseNumber, string> productIDDictionary = new Dictionary<ItemForPurchaseNumber, string>()
    {
        {ItemForPurchaseNumber.ChipsDollars099, "com.mygame.pokerhand.chipsdollars0.99"},
        {ItemForPurchaseNumber.ChipsDollars399, "com.mygame.pokerhand.chipsdollars3.99"},
        {ItemForPurchaseNumber.ChipsDollars599, "com.mygame.pokerhand.chipsdollars5.99"},
        {ItemForPurchaseNumber.ChipsDollars999, "com.mygame.pokerhand.chipsdollars9.99"},
        {ItemForPurchaseNumber.ChipsDollars1799, "com.mygame.pokerhand.chipsdollars17.99"},
        {ItemForPurchaseNumber.ChipsDollars2599, "com.mygame.pokerhand.chipsdollars25.99"},
        {ItemForPurchaseNumber.ChipsDollars4999, "com.mygame.pokerhand.chipsdollars49.99"},
        {ItemForPurchaseNumber.ChipsDollars9999, "com.mygame.pokerhand.chipsdollars99.99"},
        {ItemForPurchaseNumber.SpinDollars099, "com.mygame.pokerhand.spindollars0.99"},
        {ItemForPurchaseNumber.SpinDollars1099, "com.mygame.pokerhand.spindollars10.99"},
        {ItemForPurchaseNumber.SpinDollars2599, "com.mygame.pokerhand.spindollars25.99"},
        {ItemForPurchaseNumber.SpinDollars7999, "com.mygame.pokerhand.spindollars79.99"},
        {ItemForPurchaseNumber.MoneyBoxDollars1599, "com.mygame.pokerhand.moneyboxdollars15.99"}
    };
    private const string moneyBoxId = "com.mygame.pokerhand.moneyboxdollars15.99";
    void Start()
    {
        OnSuccessOnPayPalPurchase += OnPayPalPurchaseComplete;
        OnPurchaseCanselledPayPal += ClosePayPalWindow;
        if (m_StoreController == null)
            InitializePurchasing();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = GetComponent<Purchaser>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void ClosePayPalWindow() => webView.gameObject.SetActive(false);

    private void OpenPayPalPage(string url)
    {
        webView.Url = url;
        webView.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        OnSuccessOnPayPalPurchase -= OnPayPalPurchaseComplete;
        OnPurchaseCanselledPayPal -= ClosePayPalWindow;
    }

    public void OnPayPalPurchaseComplete(int operationCode)
    {
        switch (operationCode)
        {
            case 1: case 2: case 3: case 4:
                webView.gameObject.SetActive(false);
                OnSpinPurchased?.Invoke(operationCode - 1);
                break;
            case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                HTTP_SendGetPlayerProfile(PlayerProfileData.Instance.Id);
                webView.gameObject.SetActive(false);
                break;
            case 13:
                webView.gameObject.SetActive(false);
                MoneyBoxWindowController.Instance.OnBreakButtonClick();
                ReceiveMoneyBox(PlayerProfileData.Instance.Id);
                break;
        }
    }

    public void BuyProductIDPayPal(ItemForPurchaseNumber productID)
    {
        if (!IsInitialized)
        {
            LogError($"{DateTime.UtcNow} BuyProductID FAIL. Not initialized.");
            return;
        }
        Product product = m_StoreController.products.WithID(productIDDictionary[productID]);
        if (product == null || !product.availableToPurchase)
        {
            LogError($"{DateTime.UtcNow} BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            return;
        }
        var order = new CreateOrderViewModel()
        {
            PlayerId = PlayerProfileData.Instance.Id,
            Price = product.metadata.localizedPrice,
            Currency = product.metadata.isoCurrencyCode,
            PurchaseType = productID
        };
        var json = JsonConvert.SerializeObject(order);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Payment/paypal/create"), methodType: HTTPMethods.Post, callback: OnRequestFinished);
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.AddHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.RawData = Encoding.UTF8.GetBytes(json);
        request.Send();
        void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
            {
                var url = JsonConvert.DeserializeObject<URLDto>(resp.DataAsText);
                OpenPayPalPage(url.Value);
            }
            else 
                Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized)
            return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach(string productID in productIDDictionary.Values)
            builder.AddProduct(productID, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProductID(ItemForPurchaseNumber productID)
    {
        if (!IsInitialized)
        {
            LogError($"{DateTime.UtcNow} BuyProductID FAIL. Not initialized.");
            return;
        }
        Product product = m_StoreController.products.WithID(productIDDictionary[productID]);
        if (product == null || !product.availableToPurchase)
        {
            LogError($"{DateTime.UtcNow} BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            return;
        }
        Log(string.Format($"{DateTime.UtcNow} Purchasing product asychronously: '{0}'", product.definition.id));
        m_StoreController.InitiatePurchase(product);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Log($"{DateTime.UtcNow} Purchases are initialized");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error) => LogError("OnInitializeFailed InitializationFailureReason:" + error);

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productID = args.purchasedProduct.definition.id;
        if (spinProductsIdDictionary.ContainsKey(productID))
        {
            OnSpinPurchased?.Invoke((int)spinProductsIdDictionary[productID]);
            return PurchaseProcessingResult.Complete;
        }
        if (chipsProductsDictionary.ContainsKey(productID))
        {

            AddChips(chipsProductsDictionary[productID]);
            return PurchaseProcessingResult.Complete;
        }
        if (productID == moneyBoxId)
        {
            MoneyBoxWindowController.Instance.OnBreakButtonClick();
            ReceiveMoneyBox(PlayerProfileData.Instance.Id);
            return PurchaseProcessingResult.Complete;
        }
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(AppsFlyerConstants.PurchaseProductID, productID);
        eventValues.Add(AppsFlyerConstants.PurchaseDelay, (TimeManager.GetTime() - PlayerProfileData.Instance.RegistrationDate).ToString(AppsFlyerConstants.TimePattern));
        AppsFlyer.sendEvent(AppsFlyerConstants.Purchase, eventValues);
        return PurchaseProcessingResult.Pending;
    }

    public string GetLocalizedPrizeString(ItemForPurchaseNumber productId) 
        => m_StoreController.products.WithID(productIDDictionary[productId]).metadata.localizedPriceString;

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) => 
        LogError(string.Format("{2} OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason, DateTime.UtcNow));
}