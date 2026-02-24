using System;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    
    private void Update()
    {
        if (!_target) return;
        
        transform.position = new Vector3(_target.position.x, transform.position.y, _target.position.z);
    }
}
