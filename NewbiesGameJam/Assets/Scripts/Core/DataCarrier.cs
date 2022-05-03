using UnityEngine;
using UnityEngine.SceneManagement;

public class DataCarrier : MonoBehaviour
{
    public static DataCarrier Instance { get; private set; }

    private int _playerHp;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        _playerHp = GameManager.Instance.playerHealth.CurrentHealth;
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
        if (_playerHp <= 0)
            _playerHp = GameManager.Instance.playerHealth.StartingHealth;
        GameManager.Instance.playerHealth.CurrentHealth = _playerHp;
    }
}