using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TableWindowMoneyView : MonoBehaviour
{
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _howMuchMoneyText;
    [SerializeField] private Text _minMoneyText;
    [SerializeField] private Text _maxMoneyText;
    [SerializeField] private Button _goToMenuButton;
    [SerializeField] private Button _getMoneyButton;
    [SerializeField] private Slider _moneySlider;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;
    public UnityAction<float> OnChangedScrollValue { get; set; }
    public event Action OnPlusButtonPressed;
    public event Action OnMinusButtonPressed;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;

    private void Awake()
    {
        _moneySlider.onValueChanged.AddListener(OnChangedScrollValue);
        _moneySlider.onValueChanged.AddListener(UpdateHowMuchMoneyText);
        _goToMenuButton.onClick.AddListener(() => OnCloseButtonPressed.Invoke());
        _plusButton.onClick.AddListener(() => OnPlusButtonPressed.Invoke());
        _minusButton.onClick.AddListener(() => OnMinusButtonPressed.Invoke());
        _getMoneyButton.onClick.AddListener(() => OnEnterButton.Invoke());
    }

    private void OnEnable()
    {
        _moneySlider.value = _moneySlider.minValue;
        UpdateHowMuchMoneyText(_moneySlider.minValue);
    }

    public void UpdateMoneyScrollValue(int minValue, int maxValue)
    {
        _moneySlider.minValue = minValue; 
        _moneySlider.maxValue = maxValue;
        _minMoneyText.text = $"${minValue}";
        _maxMoneyText.text = $"${maxValue}";
    }

    public void UpdateDiscriptionText(string text) => _descriptionText.text = text; 

    public void PlusButton(int value) => _moneySlider.value += value;

    public void MinusButton(int value) => _moneySlider.value -= value;

    public void MinMoneyTextUpdate(string text) => _minMoneyText.text = text;

    public void MaxMoneyTextUpdate(string text) => _maxMoneyText.text = text;

    public void UpdateHowMuchMoneyText(float count) => _howMuchMoneyText.text = $"${(int)count}";
}