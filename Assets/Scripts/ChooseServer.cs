using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChooseServer : MonoBehaviour
{
    [SerializeField] Button selectButton;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        selectButton.onClick.AddListener(SelectButtonClick);
    }

    private void SelectButtonClick()
    {
        Hub.uriString = inputField.text;
        SceneManager.LoadScene("MainMenu");
    }
}
