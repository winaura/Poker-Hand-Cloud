using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public void SetData(ScreenData data) => UpdateScreen(data);
    protected virtual void UpdateScreen(ScreenData dat) { }
}