using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour
{
    [SerializeField] private bool onlyTriggerOnPlayer = true;
    [SerializeField] private UnityEvent onCollect;

    public void Collect()
    {
        onCollect.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onlyTriggerOnPlayer && !other.gameObject.CompareTag("Player"))
            return;
        
        Collect();
    }
}
