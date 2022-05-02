using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header ("Core")]
    public CinemachineShake cinemachineShake;
    public TimeManager timeManager;

    [Header ("Player References")]
    public GameObject player;
    public PlayerMovement playerMovement;
    public PlayerAnimator playerAnimator;
    public PlayerHealth playerHealth;

    [Header ("Grapple Gun")]
    public GameObject grappleGun;
    public GrapplingRope grapplingRope;

    [Header ("Tests")]
    [SerializeField] private bool _slowMode = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null && Instance != this)
            Destroy(gameObject);

        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        SlowMode();
    }

    private void SlowMode()
    {
        if (_slowMode)
        {
            timeManager.enabled = false;
            Time.timeScale = timeManager.SlowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        else
        {
            timeManager.enabled = true;
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Green_01")
        {
            
        }
    }
}