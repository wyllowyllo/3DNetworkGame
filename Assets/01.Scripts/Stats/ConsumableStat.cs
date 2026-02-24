using System;
using UnityEngine;

[Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _regenRate;   // 초당 재생량

    private float _value;

    public float Value     => _value;
    public float MaxValue  => _maxValue;
    public float RegenRate => _regenRate;

    public void Initialize() => _value = _maxValue;
    public void Regenerate(float deltaTime) => _value = Mathf.Min(_value + _regenRate * deltaTime, _maxValue);
    
    public void Consume(float amount) => _value = Mathf.Max(0, _value - amount);
    
    public void SetValue(float value) => _value = Mathf.Clamp(value, 0, _maxValue);
}
