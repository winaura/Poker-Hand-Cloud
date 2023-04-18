using UnityEngine;

[CreateAssetMenu(fileName = "Create Country data", menuName = "Create Country data")]
public class CountryData : ScriptableObject
{
    [SerializeField] private Sprite[] _countryData;
    public Sprite[] CountryDatas
    {
        get
        {
            return _countryData;
        }
    }
    public Sprite GetCountrySprite(int index) => _countryData[index];
}