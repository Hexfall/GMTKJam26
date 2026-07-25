using System;
using UnityEngine;

public class PassiveMovement : MonoBehaviour
{
    [SerializeField] private Vector3 direction;

    private void Update()
    {
        transform.position += direction * Time.deltaTime;
    }
}
