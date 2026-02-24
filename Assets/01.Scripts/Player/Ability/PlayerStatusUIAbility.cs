using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _staminaBar;

    private void Start()
    {
        _owner.OnHealthChanged.AddListener(UpdateHpBar);
        _owner.OnStaminaChanged.AddListener(UpdateStaminaBar);
    }

    private void UpdateHpBar()
    {
        if (_hpBar == null) return;
        
        
        _hpBar.value = _owner.CurrentHealth  /  _owner.MaxHealth;
    }

    private void UpdateStaminaBar()
    {
        if (_staminaBar == null) return;
        
        
        _staminaBar.value = _owner.CurrentStamina /  _owner.MaxStamina;
    }
}
