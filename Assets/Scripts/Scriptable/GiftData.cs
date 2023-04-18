using UnityEngine;

[CreateAssetMenu(fileName = "Create Gift Data", menuName = "Create Gift Data")]
public class GiftData : ScriptableObject
{
    [SerializeField, ArrayElementTitle("Name")] GiftInfo[] giftsInfo;
    public GiftInfo[] GiftsInfo => giftsInfo;
}