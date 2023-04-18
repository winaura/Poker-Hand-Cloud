using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent, RequireComponent(typeof(Button))]
public abstract class RatingTabBase : MonoBehaviour
{
    [SerializeField] private TMP_Text tabText;


    private RatingWindow ratingWindow;
    private Button button;

    protected abstract string LocalizationKey { get; }

    protected virtual void Awake()
    {
        ratingWindow = GetComponentInParent<RatingWindow>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => SelectTab(true));
        SetTabTexts();
    }

    private void Start()
    {
        SettingsManager.Instance.UpdateTextsEvent += SetTabTexts;
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= SetTabTexts;
    }

    private void SetTabTexts()
    {
        tabText.text = SettingsManager.Instance.GetString("RatingWindow." + LocalizationKey);
    }

    public virtual void SelectTab(bool isSelected)
    {
        button.interactable = isSelected;
        if (isSelected)
            ratingWindow.SelectTab(this);
    }

    public void ActivateTab(bool isActive)
    {
        button.interactable = isActive;
    }
}