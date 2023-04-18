using PokerHand.Common.Helpers.Player;
using System;
using UnityEngine;

public class RegistrationController : MonoBehaviour
{
    [SerializeField] RegistrationView view;
    public event Action<string, Gender, HandsSpriteType> OnRegistrationComplete;
    public event Action OnRegistrationFormClose;

    private void Start()
    {
        view.OnContinueButtonClick += OnRegistrationFormComplete;
        view.OnCloseRegistrationFormButtonClick += OnRegistrationFormCloseHandler;
    }

    private void OnDestroy()
    {
        view.OnContinueButtonClick -= OnRegistrationFormComplete;
        view.OnCloseRegistrationFormButtonClick -= OnRegistrationFormCloseHandler;
    }

    public void OnRegistrationFormComplete(string name, Gender gender, HandsSpriteType handsType) => OnRegistrationComplete?.Invoke(name, gender, handsType);

    public void OnRegistrationFormCloseHandler() => OnRegistrationFormClose?.Invoke();

    public void SetDefaultName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            view.SetNameField(name);
    }
}