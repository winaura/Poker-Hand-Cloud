using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NativePaymentText : MonoBehaviour
{
    private void Awake()
    {
        TextMeshProUGUI paymentText = GetComponent<TextMeshProUGUI>();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                paymentText.text = "Payment via Google";
                return;
            case RuntimePlatform.IPhonePlayer:
                paymentText.text = "Payment via Apple";
                return;
            case RuntimePlatform.WindowsEditor:
                paymentText.text = "Editor";
                return;
        }
    }
}
