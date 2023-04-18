using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoBehaviour : MonoBehaviour
{
    [SerializeField] private Text _worldNameText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private string _worldNameKey;
    [SerializeField] private string _descriptionKey;

    private void Start()
    {
        UpdateTexts();
        SettingsManager.Instance.UpdateTextsEvent += UpdateTexts;
    }

    private void OnDestroy() => SettingsManager.Instance.UpdateTextsEvent -= UpdateTexts;

    private void UpdateTexts()
    {
        _worldNameText.text = SettingsManager.Instance.GetString(_worldNameKey);
        _descriptionText.text = SettingsManager.Instance.GetString(_descriptionKey);
    }
}