using UnityEngine;
using StarterAssets;

public class WeaponController : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float initialChargeDuration = 3f;
    [SerializeField] private float minimumChargeDuration = 0.25f;
    [SerializeField] private float chargeReductionFactor = 0.66f;


    [Header("Opportunity Window")]
    [SerializeField] private float opportunityWindowDuration = 1f;


    [Header("Shooting")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootDistance = 100f;
    [SerializeField] private float sphereCastRadius = 0.05f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private StarterAssetsInputs input;


    [Header("Animation")]
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private float fastReloadAnimationLength = 0.967f;

    [SerializeField] private WeaponSFX weaponSFX;

    [Header("Visuals")]
    [SerializeField] private ProjectileVisual projectilePrefab;
    [SerializeField] private Transform muzzlePoint;



    private float currentChargeDuration;
    private float remainingChargeTime;

    private float remainingOpportunityTime;

    private float fastReloadTimer;


    private bool isCharging;

    private bool isOpportunityWindowActive;

    private bool isOpportunityWindowPaused;

    private bool isFastReloading;


    // Prevents starting actions while weapon is already busy
    private bool IsWeaponBusy =>
        isCharging || isFastReloading;



    private static readonly int ChargeTrigger =
        Animator.StringToHash("Charge");

    private static readonly int ShootTrigger =
        Animator.StringToHash("Shoot");

    private static readonly int MissTrigger =
        Animator.StringToHash("Miss");

    private static readonly int FastReloadTrigger =
        Animator.StringToHash("FastReload");

    private static readonly int ComboMissTrigger =
        Animator.StringToHash("ComboMiss");

    // ==========================
    // UI VALUES
    // ==========================

    public bool IsCharging => isCharging;


    public float RemainingChargeTime =>
        remainingChargeTime;


    public float CurrentChargeDuration =>
        currentChargeDuration;


    public bool IsOpportunityWindowActive =>
        isOpportunityWindowActive;


    public bool IsOpportunityWindowPaused =>
        isOpportunityWindowPaused;


    public float RemainingOpportunityTime =>
        remainingOpportunityTime;


    public float OpportunityWindowDuration =>
        opportunityWindowDuration;


    public float OpportunityProgress =>
        opportunityWindowDuration > 0
        ? remainingOpportunityTime / opportunityWindowDuration
        : 0;


    public float ChargeProgress =>
        currentChargeDuration > 0
        ? 1 - (remainingChargeTime / currentChargeDuration)
        : 0;


    public bool IsFastReloading =>
        isFastReloading;


    public float FastReloadProgress =>
        fastReloadAnimationLength > 0
        ? 1 - (fastReloadTimer / fastReloadAnimationLength)
        : 0;



    private void Start()
    {
        currentChargeDuration = initialChargeDuration;
    }



    private void Update()
    {
        UpdateCharge();

        UpdateOpportunityWindow();

        UpdateFastReload();


        if(input.firePressed)
        {
            TryStartCharge();

            input.firePressed = false;
        }
    }



    // ==========================
    // CHARGE SYSTEM
    // ==========================

    private void TryStartCharge()
    {
        // Block input while charging or fast reloading
        if(IsWeaponBusy)
            return;


        // If combo window is active,
        // use fast reload instead
        if(isOpportunityWindowActive)
        {
            StartFastReload();
            return;
        }


        isCharging = true;

        remainingChargeTime = currentChargeDuration;


        PlayChargeAnimation();
        weaponSFX.PlayReloadSound();

        Debug.Log(
            $"Charging: {currentChargeDuration}s"
        );
    }



    private void UpdateCharge()
    {
        if(!isCharging)
            return;


        remainingChargeTime -= Time.deltaTime;


        if(remainingChargeTime <= 0)
        {
            Fire();
        }
    }



    // ==========================
    // SHOOTING
    // ==========================

    private void Fire()
    {
        isCharging = false;

        remainingChargeTime = 0;

        PlayShootAnimation();
        weaponSFX.PlayShootSound();



        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f)
            );



        bool hitSomething =
            Physics.SphereCast(
                ray,
                sphereCastRadius,
                out RaycastHit hit,
                shootDistance,
                enemyLayer
            );



        Vector3 targetPoint;



        if(hitSomething)
        {
            targetPoint = hit.point;


            Debug.Log(
                "Enemy hit: " + hit.collider.name
            );


            SuccessfulHit();
        }
        else
        {
            targetPoint =
                ray.origin +
                ray.direction *
                shootDistance;


            Debug.Log("Miss");


            PlayMissAnimation();


            MissedShot();
        }



        SpawnProjectileVisual(targetPoint);
    }



    private void SuccessfulHit()
    {
        currentChargeDuration *=
            chargeReductionFactor;


        currentChargeDuration =
            Mathf.Max(
                currentChargeDuration,
                minimumChargeDuration
            );


        StartOpportunityWindow();


        Debug.Log(
            $"New charge duration: {currentChargeDuration}"
        );
    }



    private void MissedShot()
    {
        ResetCombo();


        Debug.Log(
            "Miss. Combo lost."
        );
    }



    // ==========================
    // OPPORTUNITY WINDOW
    // ==========================

    private void StartOpportunityWindow()
    {
        remainingOpportunityTime =
            opportunityWindowDuration;


        isOpportunityWindowActive = true;

        isOpportunityWindowPaused = false;
    }



    private void UpdateOpportunityWindow()
    {
        if(!isOpportunityWindowActive)
            return;


        if(isOpportunityWindowPaused)
            return;



        remainingOpportunityTime -= Time.deltaTime;



        if(remainingOpportunityTime <= 0)
        {
            ResetCombo();
        }
    }



    private void ResetCombo()
    {
        currentChargeDuration =
            initialChargeDuration;


        remainingOpportunityTime = 0;


        isOpportunityWindowActive = false;

        isOpportunityWindowPaused = false;


        PlayComboMissAnimation();


        Debug.Log(
            "Combo reset"
        );
    }



    // ==========================
    // FAST RELOAD
    // ==========================

    private void StartFastReload()
    {
        if(isFastReloading)
            return;


        isOpportunityWindowActive = false;


        fastReloadTimer =
            fastReloadAnimationLength;


        isFastReloading = true;



        PlayFastReloadAnimation();
        weaponSFX.PlayFastReloadSound();


        Debug.Log(
            "Fast reload started"
        );
    }



    private void UpdateFastReload()
    {
        if(!isFastReloading)
            return;


        fastReloadTimer -= Time.deltaTime;



        if(fastReloadTimer <= 0)
        {
            CompleteFastReload();
        }
    }



    private void CompleteFastReload()
    {
        isFastReloading = false;


        Debug.Log(
            "Fast reload complete. Shooting."
        );

        Fire();
    }



    // ==========================
    // ANIMATIONS
    // ==========================

    private void PlayChargeAnimation()
    {
        weaponAnimator.SetTrigger(
            ChargeTrigger
        );
    }



    private void PlayShootAnimation()
    {
        weaponAnimator.SetTrigger(
            ShootTrigger
        );
    }



    private void PlayMissAnimation()
    {
        Debug.Log("MISS ANIMATION TRIGGERED");
        weaponAnimator.SetTrigger(
            MissTrigger
        );
    }



    private void PlayFastReloadAnimation()
    {
        weaponAnimator.SetTrigger(
            FastReloadTrigger
        );
    }

    private void PlayComboMissAnimation()
    {
        weaponAnimator.SetTrigger(
            ComboMissTrigger
        );
    }

    // ==========================
    // PROJECTILE VISUAL
    // ==========================

    private void SpawnProjectileVisual(Vector3 target)
    {
        if(projectilePrefab == null)
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