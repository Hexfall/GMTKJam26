using UnityEngine;
using UnityEngine.Events;

public class ShootableInteractable : MonoBehaviour
{
    [SerializeField] private bool triggerOnce;
    [SerializeField] private UnityEvent onActivate;

    private bool hasTriggered;

    public void Trigger()
    {
        if(triggerOnce && hasTriggered)
            return;

        hasTriggered = true;
        onActivate.Invoke();
    }
}
