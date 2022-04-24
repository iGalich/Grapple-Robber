using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header ("Core")]
    public CameraController cameraController;
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
}