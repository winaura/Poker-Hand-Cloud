using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    private LanguageKeyKeeper languageKeeper = new LanguageKeyKeeper();
    public event Action UpdateTextsEvent;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void InitializeLanguage() => languageKeeper.SetLanguage(PlayerData.Instance.language);

    public void ChangeLanguage(Language language)
    {
        languageKeeper.SetLanguage(language);
        UpdateTextsEvent?.Invoke();
    }

    public string GetString(string key)
    {
        if (languageKeeper.stringsDictionary.ContainsKey(key))
            return languageKeeper.stringsDictionary[key];
        else
            return "not localized!!!";
    }    
}