using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IAPPayPalButton : MonoBehaviour
{
    [SerializeField] private ItemForPurchaseNumber productID;
    [SerializeField] private Text priceText;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(PurchaseClick);
        priceText.text = Purchaser.Instance.GetLocalizedPrizeString(productID);
    }

    public string GetLocalizedPrizeString() => Purchaser.Instance.GetLocalizedPrizeString(productID);

    private void PurchaseClick()
    {
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
                ChoosePaymentWindow paymentWindow = Instantiate(Resources.Load<ChoosePaymentWindow>("PaymentChoise"));
                paymentWindow.Ini((paymentChoise) =>
                {
                    switch (paymentChoise)
                    {
                        case 0:
                            Purchaser.Instance.BuyProductID(productID);
                            break;
                        case 1:
                            Purchaser.Instance.BuyProductIDPayPal(productID);
                            break;
                    }
                });
                break;
            case RuntimePlatform.IPhonePlayer:
                Purchaser.Instance.BuyProductID(productID);
                break;
            default:
                goto case RuntimePlatform.Android;
        }
    }
}
