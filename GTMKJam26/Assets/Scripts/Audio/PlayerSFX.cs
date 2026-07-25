using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Damageable))]
public class PlayerSFX : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] painClips;
    [SerializeField] private AudioClip deathClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float painVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 1f;

    [Header("Pain Pitch Randomization")]
    [SerializeField, Min(0.01f)] private float minimumPainPitch = 0.95f;
    [SerializeField, Min(0.01f)] private float maximumPainPitch = 1.05f;

    private Damageable damageable;
    private int previousPainIndex = -1;
    private bool hasPlayedDeath;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.panStereo = 0f;
        audioSource.spatialBlend = 0f;

        damageable = GetComponent<Damageable>();
        damageable.OnDamage.AddListener(PlayPainSound);
    }

    private void OnDestroy()
    {
        if (damageable != null)
            damageable.OnDamage.RemoveListener(PlayPainSound);
    }

    public void PlayPainSound()
    {
        AudioClip clip = GetRandomPainClip();

        if (clip == null)
            return;

        audioSource.pitch = Random.Range(
            Mathf.Min(minimumPainPitch, maximumPainPitch),
            Mathf.Max(minimumPainPitch, maximumPainPitch)
        );

        audioSource.PlayOneShot(clip, painVolume);
    }

    public void PlayDeathSound()
    {
        if (hasPlayedDeath || deathClip == null)
            return;

        hasPlayedDeath = true;

        var deathSoundObject = new GameObject("Player Death SFX");
        DontDestroyOnLoad(deathSoundObject);

        AudioSource deathSource = deathSoundObject.AddComponent<AudioSource>();
        deathSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        deathSource.clip = deathClip;
        deathSource.volume = deathVolume;
        deathSource.pitch = 1f;
        deathSource.panStereo = 0f;
        deathSource.spatialBlend = 0f;
        deathSource.playOnAwake = false;
        deathSource.loop = false;
        deathSource.Play();

        Destroy(deathSoundObject, deathClip.length + 0.1f);
    }

    private AudioClip GetRandomPainClip()
    {
        if (painClips == null || painClips.Length == 0)
            return null;

        int selectedIndex = Random.Range(0, painClips.Length);

        if (painClips.Length > 1)
        {
            while (selectedIndex == previousPainIndex)
                selectedIndex = Random.Range(0, painClips.Length);
        }

        previousPainIndex = selectedIndex;
        return painClips[selectedIndex];
    }
}
