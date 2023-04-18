using UnityEngine;
using UnityEngine.UI;
using System;

public class World : MonoBehaviour
{
    [SerializeField] private Worlds _worlds;
    private Button _button;
    public event Action<Worlds> OnIndexOpenedScene;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button?.onClick.AddListener(() => OnIndexOpenedScene.Invoke(_worlds)); 
    }
}