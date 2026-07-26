using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Damageable))]
public class DestructableObstacleSFX : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip destructionClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float hitVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float destructionVolume = 1f;

    [Header("Pitch Randomization")]
    [SerializeField, Min(0.01f)] private float minimumPitch = 0.95f;
    [SerializeField, Min(0.01f)] private float maximumPitch = 1.05f;

    private Damageable damageable;
    private int previousHitIndex = -1;
    private bool hasPlayedDestruction;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ConfigureAs3DPointSource(audioSource);

        damageable = GetComponent<Damageable>();
        damageable.OnDamage.AddListener(PlayHitSound);
        damageable.OnDeath.AddListener(PlayDestructionSound);
    }

    private void OnDestroy()
    {
        if (damageable == null)
            return;

        damageable.OnDamage.RemoveListener(PlayHitSound);
        damageable.OnDeath.RemoveListener(PlayDestructionSound);
    }

    public void PlayHitSound()
    {
        AudioClip clip = GetRandomHitClip();

        if (clip == null)
            return;

        PlayDetached(
            clip,
            hitVolume,
            transform.position,
            "Obstacle Hit SFX"
        );
    }

    public void PlayDestructionSound()
    {
        if (hasPlayedDestruction || destructionClip == null)
            return;

        hasPlayedDestruction = true;

        PlayDetached(
            destructionClip,
            destructionVolume,
            transform.position,
            "Obstacle Destruction SFX"
        );
    }

    private void PlayDetached(
        AudioClip clip,
        float volume,
        Vector3 position,
        string objectName)
    {
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

    private AudioClip GetRandomHitClip()
    {
        if (hitClips == null || hitClips.Length == 0)
            return null;

        int selectedIndex = Random.Range(0, hitClips.Length);

        if (hitClips.Length > 1)
        {
            while (selectedIndex == previousHitIndex)
                selectedIndex = Random.Range(0, hitClips.Length);
        }

        previousHitIndex = selectedIndex;
        return hitClips[selectedIndex];
    }

    private float GetRandomPitch()
    {
        return Random.Range(
            Mathf.Min(minimumPitch, maximumPitch),
            Mathf.Max(minimumPitch, maximumPitch)
        );
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
}
