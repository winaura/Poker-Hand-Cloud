using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _modeText;
    [SerializeField] private string _nameKey;
    [SerializeField] private string _modeKey;

    private void Start()
    {
        UpdateTexts();
        SettingsManager.Instance.UpdateTextsEvent += UpdateTexts;
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= UpdateTexts;
    }

    private void UpdateTexts()
    {
        _nameText.text = SettingsManager.Instance.GetString(_nameKey);
        _modeText.text = SettingsManager.Instance.GetString(_modeKey);
    }
}