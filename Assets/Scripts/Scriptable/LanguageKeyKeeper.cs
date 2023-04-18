using System.Collections.Generic;
using UnityEngine;

public class LanguageKeyKeeper 
{
    [HideInInspector] public Dictionary<string, string> stringsDictionary = new Dictionary<string, string>();

    public void SetLanguage(Language language)
    {
        stringsDictionary.Clear();
        string[] lines;
        switch (language)
        {
            case Language.English:
                lines = Resources.Load<TextAsset>("LanguageStrings/strings_en").text.Replace("\r", "").Split('\n');
                break;
            case Language.French:
                lines = Resources.Load<TextAsset>("LanguageStrings/strings_fr").text.Replace("\r", "").Split('\n');
                break;
            case Language.Russian:
                lines = Resources.Load<TextAsset>("LanguageStrings/strings_ru").text.Replace("\r", "").Split('\n');
                break;
            case Language.Spanish:
                lines = Resources.Load<TextAsset>("LanguageStrings/strings_sp").text.Replace("\r", "").Split('\n');
                break;
            default:
                lines = Resources.Load<TextAsset>("LanguageStrings/strings_en").text.Replace("\r", "").Split('\n');
                break;
        }
        foreach (var line in lines)
        {
            if (line.Contains(":"))
            {
                string[] str = line.Split(':');
                stringsDictionary.Add(str[0], str[1]);
            }
        }
    }
}