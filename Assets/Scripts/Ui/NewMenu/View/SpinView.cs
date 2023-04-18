using EnhancedScrollerDemos.SnappingDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class SpinView : MonoBehaviour
{
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private GameObject _spinWindow;
    [SerializeField] private Animator _rainbowAnimator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Text _winUpToText;
    [SerializeField] private Image _bordersImage;
    [SerializeField] private Text _youVeWonText;
    [SerializeField] private Text _rewardCountText;
    [SerializeField] private Text _chipsText;
    [SerializeField] private GameObject _rewardWindow;
    [SerializeField] private Text _shopText;
    [SerializeField] private Sprite[] _borderSprites;
    [SerializeField] private Color[] _spinColors;
    [SerializeField] private GameObject _rainbowParent;
    [SerializeField] private SnappingDemo _snappingDemo;
    [SerializeField] private GameObject _slotScroller;
    [SerializeField] private GameObject[] _spinGameObjects;
    [SerializeField] private Image[] _spinImage;
    [SerializeField] private Button[] _spinButtons;
    [SerializeField] private Text[] _buySpinButtonTexts;
    [SerializeField] private List<GameObject> _effectsPos;
    [SerializeField] private Color[] _colorArray;
    private List<Vector2> _startPosSpins = new List<Vector2>();
    private RarityMenuButtons _rarity;
    private State _state = State.Finally;
    public event Action OnPrepareToSpin;
    public event Action OnSpinFinally;
    private static string[] maxSlotBet = { "200K", "1M", "10M", "200M" };
    Coroutine SpinActiveCoroutine;
    private SpinView spinView;
    [SerializeField] private List<GameObject> spinList;
    [SerializeField] private Button closeSpin;
    [SerializeField] private int[] minSpinPrize = { 25000, 200000, 800000, 70000000 };

    public void SpinPurchase(int spinType)
    {
        Client.AddChips(minSpinPrize[spinType]);
        OnSpinFinally?.Invoke();
        spinView.ToSpin(spinType);
        spinList[spinType].SetActive(false);
        closeSpin.gameObject.SetActive(false);
    }

    public void CloseBuyButtons()
    {
        foreach (var spin in spinList)
            spin.SetActive(false);
    }

    private void Awake()
    {
        foreach (GameObject obj in _spinGameObjects)
            _startPosSpins.Add(obj.transform.localPosition);
        spinView = GetComponent<SpinView>();
    }

    private void SetRainbowColors(int index)
    {
        for (int i = 0; i < _rainbowParent.transform.childCount; i++)
        {
            _rainbowParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = _spinColors[index];
            _rainbowParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = _spinColors[index];
        }
    }

    private void OnEnable()
    {
        DefaultShop();
        Purchaser.Instance.OnSpinPurchased += SpinPurchase;
    }

    private void OnDisable()
    {
        DefaultShop();
        Purchaser.Instance.OnSpinPurchased -= SpinPurchase;
    }

    public void ChangeBackColor(int i) => _backGroundImage.color = _colorArray[i];

    private void DefaultShop()
    {
        for (int i = 0; i < 4; i++)
        {
            _spinGameObjects[i].transform.GetChild(0).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = SettingsManager.Instance.GetString("ShopView.Hand." + i);
            _spinGameObjects[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            _spinGameObjects[i].GetComponent<Image>().enabled = true;
            _spinGameObjects[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(310, 130);
            _spinGameObjects[i].transform.GetChild(1).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(2).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).GetComponent<Text>().text = maxSlotBet[i];
            _spinGameObjects[i].transform.GetChild(4).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(4).GetComponent<Text>().text = SettingsManager.Instance.GetString("ShopView.WinUpTo");
            _spinGameObjects[i].transform.GetChild(5).gameObject.SetActive(true);
            _spinGameObjects[i].gameObject.SetActive(true);
            _spinGameObjects[i].transform.localPosition = _startPosSpins[i];
            _spinButtons[i].gameObject.SetActive(true);
            _buySpinButtonTexts[i].text = SettingsManager.Instance.GetString("ShopView.LetsRoll");
            _spinButtons[i].transform.GetChild(0).GetComponent<Text>().text = spinList[i].GetComponent<IAPPayPalButton>().GetLocalizedPrizeString();
            _spinGameObjects[i].transform.localScale = Vector3.one;
            _slotScroller.transform.localScale = Vector3.one;
        }
        _bordersImage.gameObject.SetActive(false);
        _rewardWindow.SetActive(false);
        _winUpToText.gameObject.SetActive(false);
        _spinWindow.SetActive(false);
        _slotScroller.SetActive(false);
        _state = State.Finally;
    }

    public void ToSpin(int index)
    {
        _shopText.enabled = false;
        switch(_state)
        {
            case State.Finally:
                PrepareSpin(index);
                break;
            case State.Prepare:
                RollSpin(index);
                break;
            case State.Roll:
                StopCoroutine(SpinActiveCoroutine);
                DefaultShop();
                OnSpinFinally?.Invoke();
                break;
        }
    }

    private void RollSpin(int index)
    {
        _slotScroller.SetActive(true);
        _winUpToText.gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(1).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(2).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(5).gameObject.SetActive(false);
        _spinButtons[index].gameObject.SetActive(false);
        SpinActiveCoroutine = StartCoroutine(SpinActive(index));
        _state = State.Roll;
        _snappingDemo.PullLeverButton_OnClick();
    }

    private void PrepareSpin(int index)
    {
        OnPrepareToSpin?.Invoke();
        for (int i = 0; i < _spinGameObjects.Length; i++)
            _spinGameObjects[i].gameObject.SetActive(false);
        _winUpToText.text = SettingsManager.Instance.GetString("ShopView.WinUpTo");
        _spinGameObjects[index].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(475f, 150f);
        SetRainbowColors(index);
        _rewardWindow.SetActive(false);
        _bordersImage.sprite = _borderSprites[index];
        _bordersImage.gameObject.SetActive(true);
        _winUpToText.gameObject.SetActive(true);
        _spinWindow.SetActive(true);
        _spinGameObjects[index].GetComponent<Image>().enabled = false;
        _spinGameObjects[index].transform.GetChild(0).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(1).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(2).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(false);
        _spinGameObjects[index].transform.GetChild(5).gameObject.SetActive(false);
        _spinGameObjects[index].gameObject.SetActive(true);
        _spinGameObjects[index].transform.localPosition = new Vector2(0, 110);
        _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = spinList[index].GetComponent<IAPPayPalButton>().GetLocalizedPrizeString();
        _slotScroller.SetActive(true);
        _state = State.Prepare;
        _rarity = (RarityMenuButtons)index;
        _snappingDemo.SetRarity(_rarity);
        _slotScroller.SetActive(false);
        _spinGameObjects[index].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        _slotScroller.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    private IEnumerator SpinActive(int index)
    {
        _rainbowAnimator.SetTrigger("Start");
        AudioManager.Instance.PlaySound(Clips.Spin, _audioSource);
        yield return new WaitForSeconds(6.5f);
        AudioManager.Instance.PlaySound(Clips.Winning, _audioSource);
        _rewardWindow.GetComponent<Image>().color = _spinColors[index];
        _youVeWonText.text = SettingsManager.Instance.GetString("ShopView.YouWon");
        _chipsText.text = SettingsManager.Instance.GetString("ShopView.Chips");
        _bordersImage.gameObject.SetActive(false);
        _rewardWindow.SetActive(true);
        _spinWindow.SetActive(false);
        _spinGameObjects[index].GetComponent<Image>().enabled = false;
        _spinGameObjects[index].transform.GetChild(0).gameObject.SetActive(false);
        _state = State.Finally;
        _spinButtons[index].gameObject.SetActive(true);
        _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = SettingsManager.Instance.GetString("ShopView.Nice");
        _slotScroller.SetActive(false);
        _rewardCountText.text = _snappingDemo.GetScore().IntoCluttered();
        int extraPrize = _snappingDemo.GetScore() - minSpinPrize[index];
        if (extraPrize > 0)
        {
            Client.AddChips(extraPrize);
            OnSpinFinally?.Invoke();
        }
    }

    private enum State
    {
        Prepare,
        Roll,
        Finally
    }
}