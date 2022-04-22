using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject player;
    public PlayerMovement playerMovement;
    public PlayerAnimator playerAnimator;
    public GameObject grappleGun;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null && Instance != this)
            Destroy(gameObject);
    }
}