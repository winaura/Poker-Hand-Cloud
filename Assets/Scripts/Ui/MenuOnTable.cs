using EnhancedScrollerDemos.SnappingDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOnTable : MonoBehaviour
{
    [SerializeField] private GameObject _slotScroller;
    [SerializeField] private Text _playerMoneyText;
    [SerializeField] private Button _chipsShopButton;
    [SerializeField] private Button _bigcoinsShopButton;
    [Header("Shop"), SerializeField] private GameObject _scroller;
    [SerializeField] private GameObject _shopGameObject;
    [SerializeField] private Button _closeButton;
    [SerializeField] private List<Button> _shopButtons;
    [SerializeField] private List<Image> _buttonsImage;
    [SerializeField] private List<Sprite> _activeButtonImage;
    [SerializeField] private List<Sprite> _deactiveButtonImage;
    [SerializeField] private List<GameObject> _shopsGameObjects;
    [Header("Spin"), SerializeField] private SnappingDemo _snappingDemo;
    [SerializeField] private List<GameObject> _spinGameObjects;
    [SerializeField] private List<Image> _spinImage;
    [SerializeField] private List<Sprite> _spinDeactive;
    [SerializeField] private List<Sprite> _spinActive;
    [SerializeField] private List<Button> _spinButtons;
    [SerializeField] private List<GameObject> _effectsPos;
    private List<bool> _isActiveButtonInMenu = new List<bool> { true, false, false };
    private List<Vector2> _startPosSpins = new List<Vector2>();
    private bool _readyToRoll;
    private bool _finishRoll;
    private PlayerData _playerData;
    private RarityMenuButtons _rarity;
    Coroutine ActiveSpin;

    private void Start()
    {
        if (PlayerData.Instance != null)
            _playerData = PlayerData.Instance;
        _chipsShopButton.onClick.AddListener(OpenShop);
        _bigcoinsShopButton.onClick.AddListener(OpenShop);
        _closeButton.onClick.AddListener(DefaultShop);
        _closeButton.onClick.AddListener(CloseShop);
        _shopGameObject.SetActive(false);
        for (int i = 0; i < _spinGameObjects.Count; i++)
            _startPosSpins.Add(_spinGameObjects[i].transform.localPosition);
        DefaultShop();
        CloseShop();
    }

    private void DisableAllInMenu()
    {
        for (int i = 0; i < _shopButtons.Count; i++)
        {
            _buttonsImage[i].sprite = _deactiveButtonImage[i];
            _buttonsImage[i].SetNativeSize();
            _isActiveButtonInMenu[i] = false;
            _shopsGameObjects[i].SetActive(false);
        }
    }

    private void DefaultShop()
    {
        for (int i = 0; i < _buttonsImage.Count; i++)
        {
            _buttonsImage[i].gameObject.SetActive(true);
            _buttonsImage[i].sprite = _deactiveButtonImage[i];
            _buttonsImage[i].SetNativeSize();
            _shopsGameObjects[i].SetActive(false);
            _isActiveButtonInMenu[i] = false;
        }
        for (int i = 0; i < _spinImage.Count; i++)
        {
            _spinImage[i].sprite = _spinDeactive[i];
            _spinImage[i].transform.GetChild(0).gameObject.SetActive(false);
            _spinImage[i].transform.GetChild(0).GetComponent<Text>().text = "";
            _spinGameObjects[i].transform.GetChild(1).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(2).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).GetComponent<Text>().text = "$8000";
            _spinGameObjects[i].transform.GetChild(4).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(4).GetComponent<Text>().text = "WIN UP TO";
            _spinGameObjects[i].gameObject.SetActive(true);
            _spinGameObjects[i].transform.localPosition = _startPosSpins[i];
            _spinButtons[i].gameObject.SetActive(true);
            _spinButtons[i].transform.GetChild(0).GetComponent<Text>().text = "Let`s Roll";
            _spinGameObjects[i].transform.localScale = Vector3.one;
            _slotScroller.transform.localScale = Vector3.one;
        }
        _scroller.SetActive(false);
        _buttonsImage[0].sprite = _activeButtonImage[0];
        _buttonsImage[0].SetNativeSize();
        _shopsGameObjects[0].SetActive(true);
        _isActiveButtonInMenu[0] = true;
        _closeButton.gameObject.SetActive(true);
        _readyToRoll = false;
        _finishRoll = false;
    }

    //This method you can find in shop on spin Buttons
    public void ToSpin(int index)
    {
        if (!_readyToRoll && !_finishRoll)
        {
            for (int i = 0; i < _buttonsImage.Count; i++)
            {
                _buttonsImage[i].gameObject.SetActive(false);
                _shopsGameObjects[i].SetActive(false);
            }
            for (int i = 0; i < _spinGameObjects.Count; i++)
                _spinGameObjects[i].gameObject.SetActive(false);
            _shopsGameObjects[_shopsGameObjects.Count - 1].SetActive(true);
            _spinGameObjects[index].gameObject.SetActive(true);
            _spinGameObjects[index].transform.localPosition = new Vector2(0, 110);
            _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = "$99.99";
            _readyToRoll = true;
            _rarity = (RarityMenuButtons)index;
            _snappingDemo.SetRarity(_rarity);
            _spinGameObjects[index].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            _slotScroller.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else if (_readyToRoll && !_finishRoll)
        {
            _scroller.SetActive(true);
            _spinImage[index].sprite = _spinActive[index];
            _spinGameObjects[index].transform.GetChild(1).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(2).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(false);
            _spinButtons[index].gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            ActiveSpin = StartCoroutine(SpinActive(index));
            _readyToRoll = false;
            _snappingDemo.PullLeverButton_OnClick();
        }
        else if (!_readyToRoll && _finishRoll)
        {
            StopCoroutine(ActiveSpin);
            DefaultShop();
            ToggleWindow(2);
        }
    }

    //This method you can find on shop Buttons
    public void ToggleWindow(int Index)
    {
        if (_isActiveButtonInMenu[Index]) return;
        DisableAllInMenu();
        _buttonsImage[Index].sprite = _activeButtonImage[Index];
        _buttonsImage[Index].SetNativeSize();
        _isActiveButtonInMenu[Index] = true;
        _shopsGameObjects[Index].SetActive(true);
    }

    private void OpenShop() => _shopGameObject.SetActive(true);

    private void CloseShop() => _shopGameObject.SetActive(false);

    private IEnumerator SpinActive(int index)
    {
        yield return new WaitForSeconds(6.5f);
        _finishRoll = true;
        _spinButtons[index].gameObject.SetActive(true);
        _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = "Nice";
        _scroller.SetActive(false);
        _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(4).GetComponent<Text>().text = "You won ";
        _spinGameObjects[index].transform.GetChild(3).GetComponent<Text>().text = $"${_snappingDemo.GetScore()}";
        _spinImage[index].transform.GetChild(0).gameObject.SetActive(true);
        _spinImage[index].transform.GetChild(0).GetComponent<Text>().text = $"${_snappingDemo.GetScore()}";
        if (_playerData != null)
        {
            _playerData.money += _snappingDemo.GetScore();
            _playerMoneyText.text = $"${_playerData.money.IntoCluttered()}";
        }
        var counter = 0;
        while (counter < 3)
        {
            for (int i = 0; i < _effectsPos.Count; i++)
            {
                _effectsPos[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(0.2f);
            }
            counter++;
            yield return new WaitForSeconds(1);
        }
    }
}