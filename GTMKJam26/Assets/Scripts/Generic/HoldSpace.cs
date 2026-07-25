using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldSpace : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private UnityEvent onComplete;
    [SerializeField] private Image progressBar;
    private float time = 0f;
    public float Progress => Math.Min(time/duration, 1);
    private bool isPressed = false;

    private void Update()
    {
        if (isPressed)
            time += Time.deltaTime;
        else
            time = Math.Max(0f, time-3*Time.deltaTime);

        progressBar.fillAmount = Progress;
        
        if (!(time >= duration))
            return;
        onComplete?.Invoke();
        time = 0f;
    }

    private void OnSkip(InputValue value)
    {
        isPressed = value.isPressed;
    }
}
