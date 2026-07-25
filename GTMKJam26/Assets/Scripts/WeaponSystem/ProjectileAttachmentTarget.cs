using UnityEngine;

public class ProjectileAttachmentTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    public Transform Target =>
        target != null
        ? target
        : transform;
}
