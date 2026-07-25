using UnityEngine;
using UnityEngine.Serialization;
using StarterAssets;
using Unity.Cinemachine;

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
    [FormerlySerializedAs("enemyLayer")]
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private StarterAssetsInputs input;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float damage = 20f;


    [Header("Animation")]
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator fingerAnimator;
    [SerializeField] private float fastReloadAnimationLength = 0.967f;

    [SerializeField] private WeaponSFX weaponSFX;

    [Header("Visuals")]
    [SerializeField] private ProjectileVisual projectilePrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject chargeFingers;
    [SerializeField] private float initialImpulse = 0.1f;
    [SerializeField] private float impulseMultiplier = 1.1f;



    private float currentChargeDuration;
    private float remainingChargeTime;

    private float remainingOpportunityTime;

    private float fastReloadTimer;
    
    private float lastFireTry;


    private bool isCharging;

    private bool isOpportunityWindowActive;

    private bool isOpportunityWindowPaused;

    private bool isFastReloading;
    
    private CinemachineImpulseSource impulseSource;

    private float impulseStrength;

    private Renderer[] chargeFingerRenderers;


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
        lastFireTry = coyoteTime;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        impulseStrength = initialImpulse;

        if(chargeFingers != null)
        {
            chargeFingers.SetActive(true);

            chargeFingerRenderers =
                chargeFingers.GetComponentsInChildren<Renderer>(
                    true
                );
        }

        SetChargeFingersVisible(false);
    }



    private void Update()
    {
        lastFireTry += Time.deltaTime;
        UpdateCharge();

        UpdateOpportunityWindow();

        UpdateFastReload();

        
        if(input.firePressed)
            lastFireTry = 0;

        if(lastFireTry <= coyoteTime)
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

        SetChargeFingersVisible(true);

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

        SetChargeFingersVisible(false);
        PlayShootAnimation();
        weaponSFX.PlayShootSound();
        
        Vector2 impulseDirection = Random.insideUnitCircle.normalized;
        impulseSource.GenerateImpulseWithVelocity(impulseDirection * impulseStrength);



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
                hittableLayers
            );



        Vector3 targetPoint;
        Transform attachmentTarget = null;
        Vector3 hitNormal = -ray.direction;



        if(hitSomething)
        {
            targetPoint = hit.point;
            hitNormal = hit.normal;

            ProjectileAttachmentTarget attachmentOverride =
                hit.collider.GetComponent<
                    ProjectileAttachmentTarget
                >();

            attachmentTarget =
                attachmentOverride != null
                ? attachmentOverride.Target
                : hit.collider.transform;


            Debug.Log(
                "Shot hit: " + hit.collider.name
            );

            ShootableInteractable interactable =
                hit.collider.GetComponentInParent<
                    ShootableInteractable
                >();

            if(interactable != null)
            {
                interactable.Trigger();
            }

            SuccessfulHit(hit.collider.gameObject);
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



        SpawnProjectileVisual(
            targetPoint,
            attachmentTarget,
            hitNormal
        );
    }


    private void SuccessfulHit(GameObject other)
    {
        currentChargeDuration *=
            chargeReductionFactor;


        currentChargeDuration =
            Mathf.Max(
                currentChargeDuration,
                minimumChargeDuration
            );
        
        impulseStrength *= impulseMultiplier;


        StartOpportunityWindow();
        
        var dmg = other.GetComponentInParent<Damageable>();
        if (dmg != null)
            dmg.Damage(damage);


        Debug.Log(
            $"New charge duration: {currentChargeDuration}"
        );
    }



    private void MissedShot()
    {
        ResetCombo(false);

        Debug.Log("Miss. Combo lost.");
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
            ResetCombo(true);
        }
    }



    private void ResetCombo(bool playComboMissAnimation)
    {
        currentChargeDuration =
            initialChargeDuration;

        remainingOpportunityTime = 0;

        isOpportunityWindowActive = false;
        isOpportunityWindowPaused = false;

        if(playComboMissAnimation)
        {
            PlayComboMissAnimation();
        }

        Debug.Log("Combo reset");
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

        SetChargeFingersVisible(true);


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
    
    private void SetAnimationTrigger(int trigger)
    {
        weaponAnimator.SetTrigger(trigger);

        if(fingerAnimator != null)
        {
            fingerAnimator.SetTrigger(trigger);
        }
    }
    
    private void PlayChargeAnimation()
    {
        SetAnimationTrigger(ChargeTrigger);
    }

    private void PlayShootAnimation()
    {
        SetAnimationTrigger(ShootTrigger);
    }

    private void PlayMissAnimation()
    {
        Debug.Log("MISS ANIMATION TRIGGERED");
        SetAnimationTrigger(MissTrigger);
    }

    private void PlayFastReloadAnimation()
    {
        SetAnimationTrigger(FastReloadTrigger);
    }

    private void PlayComboMissAnimation()
    {
        SetAnimationTrigger(ComboMissTrigger);
    }

    private void SetChargeFingersVisible(bool isVisible)
    {
        if(chargeFingerRenderers == null)
            return;

        foreach(Renderer fingerRenderer in chargeFingerRenderers)
        {
            if(fingerRenderer != null)
            {
                fingerRenderer.enabled = isVisible;
            }
        }
    }

    // ==========================
    // PROJECTILE VISUAL (might be deleted)
    // ==========================

    private void SpawnProjectileVisual(
        Vector3 target,
        Transform attachmentTarget,
        Vector3 hitNormal
    )
    {
        if(projectilePrefab == null)
            return;
        
        ProjectileVisual projectile =
            Instantiate(
                projectilePrefab,
                muzzlePoint.position,
                muzzlePoint.rotation
            );


        projectile.Initialize(
            target,
            attachmentTarget,
            hitNormal
        );
    }
}
