using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _audioSource;
    private AudioSource _musicSource;

    [SerializeField] private AudioClip _tutorialMusic;
    [SerializeField] private AudioClip _greenMusic;
    [SerializeField] private AudioClip _blueMusic;
    [SerializeField] private AudioClip _redMusic;
    [SerializeField] private AudioClip _bossMusic;

    public AudioSource MusicSource { get => _musicSource; set => _musicSource = value; }

    private void Awake() 
    {
        _audioSource = GetComponent<AudioSource>();
        _musicSource = GetComponentInChildren<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null && Instance != this)
            Destroy(this.gameObject);
    }

    public void PlaySound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        switch (levelIndex)
        {
            case 0:
                _musicSource.clip = _tutorialMusic;
                _musicSource.Play();
                break;
            case 3:
                _musicSource.clip = _greenMusic;
                _musicSource.Play();
                break;
            case 6:
                _musicSource.clip = _blueMusic;
                _musicSource.Play();
                break;
            case 9:
                _musicSource.clip = _redMusic;
                _musicSource.Play();
                break;
            case 12:
                _musicSource.clip = _bossMusic;
                break;
        }
    }
}