using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour
{
    [SerializeField] private bool onlyTriggerOnPlayer = true;
    [SerializeField] private AudioClip collectSfx;
    [SerializeField, Range(0f, 1f)] private float collectSfxVolume = 1f;
    [SerializeField] private UnityEvent onCollect;

    private bool hasBeenCollected;

    public void Collect()
    {
        if (hasBeenCollected)
            return;

        hasBeenCollected = true;

        if (collectSfx != null)
            AudioSource.PlayClipAtPoint(
                collectSfx,
                transform.position,
                collectSfxVolume
            );

        onCollect.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onlyTriggerOnPlayer && !other.gameObject.CompareTag("Player"))
            return;
        
        Collect();
    }
}
