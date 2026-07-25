using UnityEngine;
using UnityEngine.Events;

public class DoXTimes : MonoBehaviour
{
    [SerializeField] private int target;
    [SerializeField] private UnityEvent onComplete;

    public void Trigger()
    {
        target--;
        if (target == 0)
            onComplete.Invoke();
    }
}
