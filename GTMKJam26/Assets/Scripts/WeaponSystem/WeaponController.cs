using UnityEngine;
using StarterAssets;

public class WeaponController : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float initialChargeDuration = 3f;
    [SerializeField] private float minimumChargeDuration = 0.25f;
    [SerializeField] private float chargeReductionFactor = 0.66f;

    [Header("Shooting")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootDistance = 100f;
    [SerializeField] private float sphereCastRadius = 0.05f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private StarterAssetsInputs input;
    
    [Header("Visuals")]
    [SerializeField] private ProjectileVisual projectilePrefab;
    [SerializeField] private Transform muzzlePoint;

    private float currentChargeDuration;
    private float remainingChargeTime;

    private bool isCharging;
    
    private bool previousFireState;

    // Public values for UI reticule
    public bool IsCharging => isCharging;

    public float RemainingChargeTime => remainingChargeTime;

    public float CurrentChargeDuration => currentChargeDuration;

    public float ChargeProgress =>
        1 - (remainingChargeTime / currentChargeDuration);


    private void Start()
    {
        currentChargeDuration = initialChargeDuration;
    }
    
    private void Update()
    {
        UpdateCharge();

        if(input.firePressed)
        {
            TryStartCharge();
            input.firePressed = false;
        }
        
        previousFireState = input.fire;
    }


    private void TryStartCharge()
    {
        if (isCharging)
            return;
        
        isCharging = true;
        remainingChargeTime = currentChargeDuration;

        Debug.Log($"Shot charging: {currentChargeDuration}s");
    }


    private void UpdateCharge()
    {
        if (!isCharging)
            return;


        remainingChargeTime -= Time.deltaTime;


        if (remainingChargeTime <= 0)
        {
            Fire();
        }
    }


    private void Fire()
    {
        isCharging = false;
        remainingChargeTime = 0;
        
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f)
        );


        bool hitSomething = Physics.SphereCast(
            ray,
            sphereCastRadius,
            out RaycastHit hit,
            shootDistance,
            enemyLayer
        );


        Vector3 targetPoint;


        if (hitSomething)
        {
            targetPoint = hit.point;

            Debug.Log("Enemy hit: " + hit.collider.name);

            SuccessfulHit();
        }
        else
        {
            targetPoint = ray.origin + ray.direction * shootDistance;

            Debug.Log("Miss");

            MissedShot();
        }


        SpawnProjectileVisual(targetPoint);
    }


    private void SuccessfulHit()
    {
        currentChargeDuration *= chargeReductionFactor;

        currentChargeDuration = Mathf.Max(
            currentChargeDuration,
            minimumChargeDuration
        );

        Debug.Log(
            $"New charge duration: {currentChargeDuration}"
        );
    }


    private void MissedShot()
    {
        currentChargeDuration = initialChargeDuration;

        Debug.Log(
            "Charge reset"
        );
    }


    private void SpawnProjectileVisual(Vector3 target)
    {
        if (projectilePrefab == null)
            return;


        ProjectileVisual projectile =
            Instantiate(
                projectilePrefab,
                muzzlePoint.position,
                Quaternion.identity
            );


        projectile.Initialize(target);
    }
}
