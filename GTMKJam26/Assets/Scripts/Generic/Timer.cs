using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private bool startOnLoad;
    [SerializeField] private bool repeats;
    [SerializeField] private float duration = 10f;
    [SerializeField] private  UnityEvent onTimerEnd;
    
    private bool active = false;
    private float resetDuration;

    private void Start()
    {
        resetDuration = duration;
        if (startOnLoad)
            StartTimer();
    }

    private void Update()
    {
        if (!active)
            return;

        duration -= Time.deltaTime;
        if  (duration > 0)
            return;

        active = repeats;
        duration = resetDuration;
        onTimerEnd.Invoke();
    }

    public void StartTimer()
    {
        active = true;
    }
    
    public void RestartTimer()
    {
        duration = resetDuration;
        StartTimer();
    }

    public void StopTimer()
    {
        active = false;
        duration = resetDuration;
    }

    public void PauseTimer()
    {
        active = false;
    }
}
