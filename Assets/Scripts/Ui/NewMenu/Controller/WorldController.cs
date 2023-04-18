using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private World[] _worldsButton;
    [SerializeField] private GameObject levelsObject;
    [SerializeField] private ScrollViewTables _scrollViewTables;

    private void Start()
    {        
        foreach (var world in _worldsButton)        
            world.OnIndexOpenedScene += OpenWorld;
        OpenStartScreen();
    }

    private void OpenWorld(Worlds obj)
    {
        levelsObject.SetActive(true);
        _scrollViewTables.SetWorld((int)obj);
        GameModeSettings.Instance.world = obj;
    }

    private void OpenStartScreen()
    {
        Worlds worldIndex = GameModeSettings.Instance.world;
        if (worldIndex == Worlds.None)
            OpenMainMenu();
        else
            OpenWorld((int)worldIndex);
    }

    public void CloseAllWorlds() => levelsObject.SetActive(false);

    public void OpenWorld(int obj)
    {
        levelsObject.SetActive(true);
        _scrollViewTables.SetWorld(obj);
    }

    public void OpenMainMenu()
    {
        CloseAllWorlds();
        _mainMenu.SetActive(true);
    }
    public void SaveWorldIndex(int index) => GameModeSettings.Instance.world = (Worlds)index;
}