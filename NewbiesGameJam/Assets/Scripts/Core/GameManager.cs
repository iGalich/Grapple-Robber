using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerMovement playerMovement;
    public GameObject player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null && Instance != this)
            Destroy(gameObject);
    }
}