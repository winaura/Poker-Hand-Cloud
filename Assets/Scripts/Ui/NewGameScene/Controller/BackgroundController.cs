using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private BackgroundView _view;
    [SerializeField] private TableData _data;

    private void Start() => _view.SetBackground(_data.GetTableSprite((int)GameModeSettings.Instance.TablesInWorlds));
}
