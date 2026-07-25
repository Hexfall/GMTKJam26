using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemySFX : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] alertClips;
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] attackHitPlayerClips;
    [SerializeField] private AudioClip[] painClips;
    [SerializeField] private AudioClip[] sizzleClips;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip[] movementClips;

    [Header("Movement")]
    [SerializeField, Min(0.05f)] private float minimumMovementInterval = 0.4f;
    [SerializeField, Min(0.05f)] private float maximumMovementInterval = 0.65f;
    [SerializeField, Min(0f)] private float minimumMovementSpeed = 0.1f;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float alertVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float attackVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float attackHitPlayerVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float painVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float sizzleVolume = 0.6f;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float movementVolume = 0.5f;

    [Header("Pitch Randomization")]
    [SerializeField, Min(0.01f)] private float minimumPitch = 0.95f;
    [SerializeField, Min(0.01f)] private float maximumPitch = 1.05f;

    private NavMeshAgent agent;
    private float movementTimer;
    private bool wasMoving;
    private int previousAlertIndex = -1;
    private int previousAttackIndex = -1;
    private int previousAttackHitPlayerIndex = -1;
    private int previousPainIndex = -1;
    private int previousSizzleIndex = -1;
    private int previousDeathIndex = -1;
    private int previousMovementIndex = -1;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        agent = GetComponent<NavMeshAgent>();
        ConfigureAs3DPointSource(audioSource);
    }

    private void Update()
    {
        bool isMoving = IsMoving();

        if (!isMoving)
        {
            wasMoving = false;
            return;
        }

        if (!wasMoving)
        {
            movementTimer = Random.Range(0f, GetMovementInterval());
            wasMoving = true;
        }

        movementTimer -= Time.deltaTime;

        if (movementTimer > 0f)
            return;

        PlayRandomOneShot(
            movementClips,
            movementVolume,
            ref previousMovementIndex
        );

        movementTimer = GetMovementInterval();
    }

    public void PlayAttackSound()
    {
        PlayRandomOneShot(
            attackClips,
            attackVolume,
            ref previousAttackIndex
        );
    }

    public void PlayAlertSound()
    {
        PlayRandomOneShot(
            alertClips,
            alertVolume,
            ref previousAlertIndex
        );
    }

    public void PlayAttackHitPlayerSound(Vector3 playerPosition)
    {
        PlayRandomAtPosition(
            attackHitPlayerClips,
            attackHitPlayerVolume,
            playerPosition,
            ref previousAttackHitPlayerIndex,
            "Enemy Attack Hit SFX"
        );
    }

    public void PlayPainSound()
    {
        PlayRandomOneShot(
            painClips,
            painVolume,
            ref previousPainIndex
        );

        PlayRandomOneShot(
            sizzleClips,
            sizzleVolume,
            ref previousSizzleIndex
        );
    }

    public void PlayDeathSound()
    {
        PlayRandomAtPosition(
            deathClips,
            deathVolume,
            transform.position,
            ref previousDeathIndex,
            "Enemy Death SFX"
        );
    }

    private bool IsMoving()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return false;

        return agent.velocity.sqrMagnitude >
               minimumMovementSpeed * minimumMovementSpeed;
    }

    private float GetMovementInterval()
    {
        float minimum = Mathf.Min(
            minimumMovementInterval,
            maximumMovementInterval
        );

        float maximum = Mathf.Max(
            minimumMovementInterval,
            maximumMovementInterval
        );

        return Random.Range(minimum, maximum);
    }

    private void PlayRandomOneShot(
        AudioClip[] clips,
        float volume,
        ref int previousIndex)
    {
        AudioClip clip = GetRandomClip(clips, ref previousIndex);

        if (clip == null)
            return;

        audioSource.pitch = GetRandomPitch();
        audioSource.PlayOneShot(clip, volume);
    }

    private void PlayRandomAtPosition(
        AudioClip[] clips,
        float volume,
        Vector3 position,
        ref int previousIndex,
        string objectName)
    {
        AudioClip clip = GetRandomClip(clips, ref previousIndex);

        if (clip == null)
            return;

        var soundObject = new GameObject(objectName);
        soundObject.transform.position = position;

        AudioSource detachedSource = soundObject.AddComponent<AudioSource>();
        detachedSource.outputAudioMixerGroup =
            audioSource.outputAudioMixerGroup;
        detachedSource.priority = audioSource.priority;
        detachedSource.rolloffMode = audioSource.rolloffMode;
        detachedSource.minDistance = audioSource.minDistance;
        detachedSource.maxDistance = audioSource.maxDistance;

        if (audioSource.rolloffMode == AudioRolloffMode.Custom)
        {
            detachedSource.SetCustomCurve(
                AudioSourceCurveType.CustomRolloff,
                audioSource.GetCustomCurve(
                    AudioSourceCurveType.CustomRolloff
                )
            );
        }

        ConfigureAs3DPointSource(detachedSource);

        detachedSource.clip = clip;
        detachedSource.volume = volume;
        detachedSource.pitch = GetRandomPitch();
        detachedSource.Play();

        float playbackDuration =
            clip.length / Mathf.Max(Mathf.Abs(detachedSource.pitch), 0.01f);
        Destroy(soundObject, playbackDuration + 0.1f);
    }

    private static void ConfigureAs3DPointSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.panStereo = 0f;
        source.spatialBlend = 1f;
        source.spread = 0f;
        source.spatialize = false;
        source.dopplerLevel = 0f;
    }

    private AudioClip GetRandomClip(
        AudioClip[] clips,
        ref int previousIndex)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int selectedIndex = Random.Range(0, clips.Length);

        if (clips.Length > 1)
        {
            while (selectedIndex == previousIndex)
                selectedIndex = Random.Range(0, clips.Length);
        }

        previousIndex = selectedIndex;
        return clips[selectedIndex];
    }

    private float GetRandomPitch()
    {
        return Random.Range(
            Mathf.Min(minimumPitch, maximumPitch),
            Mathf.Max(minimumPitch, maximumPitch)
        );
    }
}
