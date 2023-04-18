using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ShopWindowType { Default = -1, Chips, BigCoins, Spin }

[RequireComponent(typeof(Canvas))]
public class ShopView : MonoBehaviour
{
    [SerializeField] private Text _shopText;
    [SerializeField] private List<Text> _buttonsTexts;
    [SerializeField] private List<Outline> _buttonsTextsOutline;
    [SerializeField] private List<Image> _buttonsImage;
    [SerializeField] private List<GameObject> _shopsGameObjects;
    [SerializeField] private Color _activeOutlineTextColor;
    [SerializeField] private Color _deactiveOutlineTextColor;
    [SerializeField] private GameObject _notEnoughMoney;
    [SerializeField] private Canvas _shopCanvas;
    [SerializeField] private Button _closeSpinButton;
    [SerializeField] private Button _closeShopCanvasButton;
    [SerializeField] private GameObject[] _buyButtonsArray;
    private int _activeButton = -1;
    public bool isMoneyEnough = false;

    private void OnEnable() => DefaultShop();

    private void Awake()
    {
        _closeSpinButton.onClick.AddListener(() => ToggleWindow(1));
        _closeShopCanvasButton.onClick.AddListener(() => DestroyShop());
    }

    private void DestroyShop() => Destroy(gameObject);

    private void DeactivteBuyButtons(GameObject[] gameObjects)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i].SetActive(false);
    }

    public void SetMainCamera(Camera camera)
    {
        _shopCanvas.worldCamera = camera;
        _shopCanvas.planeDistance = 25;
    }

    private void DefaultShop()
    {
        _shopText.enabled = true;
        ShowText();
        for (int i = 0; i < _buttonsTexts.Count; i++)
        {
            _buttonsTexts[i].gameObject.SetActive(true);
            _buttonsTextsOutline[i].effectColor = _deactiveOutlineTextColor;
            _buttonsTexts[i].rectTransform.sizeDelta = new Vector2(200f, 50f);
            _buttonsTexts[i].SetNativeSize();
            _shopsGameObjects[i].SetActive(false);
        }
        _buttonsImage[0].gameObject.SetActive(true);
    }

    private void ShowText()
    {
        if (isMoneyEnough)
        {
            DontDestroyOnLoad(gameObject);
            _notEnoughMoney.SetActive(true);
            StartCoroutine(NotEnoughText());
        }
    }

    public void ToSpin()
    {
        _closeSpinButton.gameObject.SetActive(true);
        _closeShopCanvasButton.gameObject.SetActive(false);
        for (int i = 0; i < _buttonsImage.Count; i++)
        {
            _buttonsImage[i].gameObject.SetActive(false);
            _shopsGameObjects[i].gameObject.SetActive(false);
        }
        _shopsGameObjects[_shopsGameObjects.Count - 1].SetActive(true);
    }

    public void ShowAllElements()
    {
        DefaultShop();
        ToggleWindow(_buttonsImage.Count - 1);
    }

    public void ToggleWindow(int index)
    {
        DefaultShop();
        _activeButton = index;
        _buttonsTextsOutline[index].effectColor = _activeOutlineTextColor;
        _buttonsTexts[index].rectTransform.sizeDelta = new Vector2(300f, 100f);
        _buttonsTexts[index].SetNativeSize();
        _shopsGameObjects[index].SetActive(true);
    }
    public void UpdateTexts()
    {
        _shopText.text = SettingsManager.Instance.GetString("ShopView.Shop");
        _buttonsTexts[0].text = SettingsManager.Instance.GetString("ShopView.Chips");
        _buttonsTexts[1].text = SettingsManager.Instance.GetString("ShopView.Spin");
    }

    IEnumerator NotEnoughText()
    {
        yield return new WaitForSeconds(2);
        _notEnoughMoney.SetActive(false);
        StopCoroutine(NotEnoughText());
    }
}