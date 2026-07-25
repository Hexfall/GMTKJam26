using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class ElevatorPassengerTrigger : MonoBehaviour
{
    [SerializeField] private Transform elevatorTransform;

    private readonly HashSet<Collider> passengerColliders =
        new HashSet<Collider>();

    private Transform passenger;
    private Transform originalParent;

    private void Awake()
    {
        if(elevatorTransform == null)
        {
            elevatorTransform =
                transform.parent != null
                ? transform.parent
                : transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController controller =
            other.GetComponentInParent<FirstPersonController>();

        if(controller == null)
            return;

        Transform enteringPassenger =
            controller.transform;

        if(
            passenger != null &&
            passenger != enteringPassenger
        )
        {
            return;
        }

        passengerColliders.Add(other);

        if(passenger != null)
            return;

        passenger = enteringPassenger;
        originalParent = passenger.parent;

        passenger.SetParent(
            elevatorTransform,
            true
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if(!passengerColliders.Remove(other))
            return;

        if(passengerColliders.Count == 0)
        {
            DetachPassenger();
        }
    }

    private void DetachPassenger()
    {
        passengerColliders.Clear();

        if(passenger == null)
            return;

        passenger.SetParent(
            originalParent,
            true
        );

        passenger = null;
        originalParent = null;
    }
}
