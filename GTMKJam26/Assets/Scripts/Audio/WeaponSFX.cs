using UnityEngine;

public class WeaponSFX : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource reloadAudioSource;
    [SerializeField] private AudioSource shootAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip fastReloadClip;
    [SerializeField] private AudioClip[] shootClips;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    private float reloadVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float shootVolume = 1f;

    [Header("Shoot Randomization")]
    [SerializeField] private float minimumShootPitch = 0.97f;
    [SerializeField] private float maximumShootPitch = 1.03f;

    private int previousShootIndex = -1;

    private void Awake()
    {
        reloadAudioSource.playOnAwake = false;
        shootAudioSource.playOnAwake = false;
    }

    public void PlayReloadSound()
    {
        if (reloadClip == null)
            return;

        // Replaces only the previous reload sound.
        reloadAudioSource.Stop();
        reloadAudioSource.clip = reloadClip;
        reloadAudioSource.pitch = 1f;
        reloadAudioSource.volume = reloadVolume;
        reloadAudioSource.Play();
    }

    public void PlayFastReloadSound()
    {
        if (fastReloadClip == null)
            return;

        reloadAudioSource.Stop();
        reloadAudioSource.clip = fastReloadClip;
        reloadAudioSource.pitch = 1f;
        reloadAudioSource.volume = reloadVolume;
        reloadAudioSource.Play();
    }

    public void PlayShootSound()
    {
        // Stops only the reload sound.
        reloadAudioSource.Stop();

        if (shootClips == null || shootClips.Length == 0)
            return;

        int selectedIndex = Random.Range(0, shootClips.Length);

        if (shootClips.Length > 1)
        {
            while (selectedIndex == previousShootIndex)
            {
                selectedIndex = Random.Range(0, shootClips.Length);
            }
        }

        previousShootIndex = selectedIndex;

        shootAudioSource.pitch =
            Random.Range(minimumShootPitch, maximumShootPitch);

        shootAudioSource.PlayOneShot(
            shootClips[selectedIndex],
            shootVolume
        );
    }
}