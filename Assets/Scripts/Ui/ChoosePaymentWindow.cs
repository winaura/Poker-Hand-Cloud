using System;
using TMPro;
using UnityEngine;

public class ChoosePaymentWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ChoiseText;
    private Action<int> callback;

    private void Awake() => ChoiseText.text = SettingsManager.Instance.GetString("ShopView.ChoosePayment");

    public void Ini(Action<int> callback) => this.callback = callback;

    public void Choose(int choise)
    {
        callback(choise);
        Close();
    }

    public void Close() => Destroy(gameObject);
}