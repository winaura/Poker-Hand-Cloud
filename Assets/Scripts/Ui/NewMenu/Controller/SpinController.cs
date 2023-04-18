using System;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    [SerializeField] private SpinView _spinView;
    [SerializeField] private ShopView _shopView;
    public event Action OnPrepareToSpin;
    public event Action OnSpinFinally;

    private void OnEnable()
    {
        _spinView.OnPrepareToSpin += _shopView.ToSpin;
        if(PlayerPanelController.Instance!=null)
            _spinView.OnSpinFinally += PlayerPanelController.Instance.MP_UpdateTotalMoney;
    }

    private void OnDisable()
    {
        _spinView.OnPrepareToSpin -= _shopView.ToSpin;
        _spinView.OnSpinFinally -= PlayerPanelController.Instance.MP_UpdateTotalMoney;
    }
}