using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private const string MainMenuSceneName = "Main Menu";
    private const string GameplaySceneName = "FPSController";

    private static MusicManager instance;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private AudioSource musicSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = volume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void StopMusic()
    {
        if (instance != null)
            instance.StopCurrentMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        PlayMusicForScene(scene);
    }

    private void PlayMusicForScene(Scene scene)
    {
        if (scene.name == MainMenuSceneName)
        {
            Play(mainMenuMusic);
            return;
        }

        if (scene.name == GameplaySceneName)
        {
            Play(gameplayMusic);
            return;
        }

        StopCurrentMusic();
    }

    private void Play(AudioClip clip)
    {
        if (clip == null)
        {
            StopCurrentMusic();
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    private void StopCurrentMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }
}
