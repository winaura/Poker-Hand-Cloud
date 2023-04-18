using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private Camera _mainCamera;
    private ShopView _shopView;
    public static bool wasLackOfMoney = false;

    private void Awake()
    {
        if (wasLackOfMoney)
        {
            wasLackOfMoney = false;
            OpenWindow();
        }
    }

    public void OpenWindow(int page = 0)
    {
        var shop = Instantiate(shopPrefab);
        _shopView = shop.GetComponent<ShopView>();
        _shopView.ToggleWindow(page);
        _shopView.UpdateTexts();
        _shopView.SetMainCamera(_mainCamera);
    }

    public void OpenWindow(bool isEnoughMoney, int page = 0)
    {
        var shop = Instantiate(shopPrefab);
        _shopView = shop.GetComponent<ShopView>();
        _shopView.isMoneyEnough = isEnoughMoney;
        _shopView.ToggleWindow(page);
        _shopView.UpdateTexts();
    }

    public void ToSpin() => _shopView.ToSpin();

    public void ShowAllElemnt() => _shopView.ShowAllElements();
}