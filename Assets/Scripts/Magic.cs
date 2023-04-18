using UnityEngine;
using UnityEngine.UI;

public class Magic : MonoBehaviour
{
    [SerializeField] bool isActive = false;
    [SerializeField] GameObject testObj;
    [SerializeField] Text log;
    [SerializeField] RawImage image;
    static Magic _instance;
    static public Magic Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _instance.log.text = string.Empty;
        testObj.SetActive(isActive);
    }

    public static void Log(string line) => _instance.log.text += $"{line}\n";

    public static void LogImage(Texture2D texture) => _instance.image.texture = texture;
}