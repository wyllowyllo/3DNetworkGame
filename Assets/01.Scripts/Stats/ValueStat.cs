using System;
using UnityEngine;

[Serializable]
public class ValueStat
{
    [SerializeField] private float _value;
    public float Value => _value;

    public void Increase(float amount) => _value += amount;
    public void Decrease(float amount) => _value -= amount;
    public void SetValue(float value)  => _value = value;
}
