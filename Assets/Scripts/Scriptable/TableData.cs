using UnityEngine;

[CreateAssetMenu(fileName = "Create Table data", menuName = "Create Table data")]
public class TableData : ScriptableObject
{
    [SerializeField] private Sprite[] _tableData;

    public Sprite GetTableSprite(int index) => _tableData[index];
}
