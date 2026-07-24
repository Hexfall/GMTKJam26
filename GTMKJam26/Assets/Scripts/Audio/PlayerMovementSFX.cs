using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovementSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip[] jumpClips;
    [SerializeField] private AudioClip[] landingClips;

    [Header("Footsteps")]
    [SerializeField] private float walkingFootstepInterval = 0.5f;
    [SerializeField] private float runningFootstepInterval = 0.3f;
    [SerializeField] private float startRunningThreshold = 5f;
    [SerializeField] private float stopRunningThreshold = 4.5f;
    [SerializeField] private float minimumMovementSpeed = 0.1f;

    [Header("Landing")]
    [Tooltip("Player must be airborne for at least this long before landing audio plays.")]
    [SerializeField] private float minimumAirTime = 0.15f;

    [Header("Randomization")]
    [SerializeField] private float minimumPitch = 0.95f;
    [SerializeField] private float maximumPitch = 1.05f;

    [SerializeField, Range(0f, 1f)]
    private float footstepVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float jumpVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float landingVolume = 1f;

    private CharacterController characterController;
    private AudioSource audioSource;

    private bool wasGrounded;
    private float footstepTimer;
    private float airTime;

    private int previousFootstepIndex = -1;
    private int previousJumpIndex = -1;
    private int previousLandingIndex = -1;

    private bool wasRunning;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        wasGrounded = characterController.isGrounded;
    }

    private void Update()
    {
        bool isGrounded = characterController.isGrounded;

        HandleJumpAndLanding(isGrounded);
        HandleFootsteps(isGrounded);

        wasGrounded = isGrounded;
    }

    private void HandleFootsteps(bool isGrounded)
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float horizontalSpeed = horizontalVelocity.magnitude;
        bool isMoving = horizontalSpeed > minimumMovementSpeed;

        if (!isGrounded || !isMoving)
        {
            footstepTimer = 0f;
            wasRunning = false;
            return;
        }

        bool isRunning = wasRunning
            ? horizontalSpeed >= stopRunningThreshold
            : horizontalSpeed >= startRunningThreshold;

        float currentInterval = isRunning
            ? runningFootstepInterval
            : walkingFootstepInterval;

        if (isRunning && !wasRunning)
        {
            footstepTimer = 0f;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            PlayRandomClip(
                footstepClips,
                footstepVolume,
                ref previousFootstepIndex
            );

            footstepTimer = currentInterval;
        }

        wasRunning = isRunning;
    }

    private void HandleJumpAndLanding(bool isGrounded)
    {
        if (wasGrounded && !isGrounded)
        {
            airTime = 0f;

            PlayRandomClip(
                jumpClips,
                jumpVolume,
                ref previousJumpIndex
            );
        }

        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }

        // Landed
        if (!wasGrounded && isGrounded)
        {
            if (airTime >= minimumAirTime)
            {
                PlayRandomClip(
                    landingClips,
                    landingVolume,
                    ref previousLandingIndex
                );
            }

            airTime = 0f;
        }
    }

    private void PlayRandomClip(
        AudioClip[] clips,
        float volume,
        ref int previousIndex)
    {
        if (clips == null || clips.Length == 0)
            return;

        int selectedIndex = Random.Range(0, clips.Length);

        // Avoid playing the same sound twice in a row.
        if (clips.Length > 1)
        {
            while (selectedIndex == previousIndex)
            {
                selectedIndex = Random.Range(0, clips.Length);
            }
        }

        previousIndex = selectedIndex;

        audioSource.pitch = Random.Range(minimumPitch, maximumPitch);
        audioSource.PlayOneShot(clips[selectedIndex], volume);
    }
}