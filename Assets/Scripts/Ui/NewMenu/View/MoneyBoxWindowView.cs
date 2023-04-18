using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoneyBoxWindowView : MonoBehaviour
{
    [SerializeField] private Animator _safeAnimator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Button _breakButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _breakButtonText;
    [SerializeField] private Image _moneyBoxImage;
    [SerializeField] private Sprite _startSprite;
    [SerializeField] private CollectChipsAnimation _collectAnimation;
    [SerializeField] private Text _openText;
    public Action OnCloseButtonClick;

    public void Awake() => _closeButton.onClick.AddListener(() => OnCloseButtonClick?.Invoke());

    private void OnEnable()
    {
        _openText.text = SettingsManager.Instance.GetString("MoneyBox.Open");
        _openText.gameObject.SetActive(true);
    }

    public void BreakMoneyBox()
    {
        _openText.gameObject.SetActive(false);
        _safeAnimator.SetTrigger("OpenSafe");
        AudioManager.Instance.PlaySound(Clips.MoneyBoxChips, _audioSource, 1.2f);
        StartCoroutine(CollectRewardDelay(3f));
        _breakButton.gameObject.SetActive(false);
        _closeButton.gameObject.SetActive(false);
    }

    private IEnumerator CollectRewardDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _collectAnimation.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _moneyBoxImage.sprite = _startSprite;
        _breakButton.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(true);
        _collectAnimation.gameObject.SetActive(false);
    }
}