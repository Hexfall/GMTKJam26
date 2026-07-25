using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEnter : MonoBehaviour
{
    [SerializeField] private bool playerOnly = true;
    [SerializeField] private UnityEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (playerOnly && !other.CompareTag("Player"))
            return;
        
        onTriggerEnter.Invoke();
    }
}
