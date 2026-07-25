using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    private Vector3 target;
    private Transform attachmentTarget;
    private Vector3 localTarget;
    private Vector3 localHitNormal;
    private bool isInitialized;

    [SerializeField] private float speed = 40f;
    [SerializeField] private float arrivalDistance = 0.05f;
    [SerializeField] private float surfaceOffset = 0.01f;
    [SerializeField] private Vector3 flightRotationOffset;
    [SerializeField] private Vector3 stuckRotationOffset;


    public void Initialize(
        Vector3 destination,
        Transform targetTransform,
        Vector3 surfaceNormal
    )
    {
        isInitialized = true;
        target = destination;
        attachmentTarget = targetTransform;

        if(attachmentTarget != null)
        {
            localTarget =
                attachmentTarget.InverseTransformPoint(destination);

            localHitNormal =
                attachmentTarget.InverseTransformDirection(
                    surfaceNormal
                );
        }

        FaceTarget(destination);
    }


    private void Update()
    {
        if(!isInitialized)
            return;

        Vector3 currentTarget = target;

        if(attachmentTarget != null)
        {
            currentTarget =
                attachmentTarget.TransformPoint(localTarget);

            target = currentTarget;
        }

        FaceTarget(currentTarget);

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );


        if(Vector3.Distance(
            transform.position,
            currentTarget
        ) <= arrivalDistance)
        {
            Arrive(currentTarget);
        }
    }


    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction =
            destination - transform.position;

        if(direction.sqrMagnitude <= 0.0001f)
            return;

        transform.rotation =
            Quaternion.LookRotation(direction.normalized) *
            Quaternion.Euler(flightRotationOffset);
    }


    private void Arrive(Vector3 destination)
    {
        if(attachmentTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 hitNormal =
            attachmentTarget.TransformDirection(
                localHitNormal
            ).normalized;

        transform.position =
            destination +
            hitNormal * surfaceOffset;

        transform.rotation =
            Quaternion.LookRotation(-hitNormal) *
            Quaternion.Euler(stuckRotationOffset);

        transform.SetParent(
            attachmentTarget,
            true
        );

        enabled = false;
    }
}
