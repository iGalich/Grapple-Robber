using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Sprite[] _healthSprites;
    [SerializeField] private Sprite _deathSprite;
    private PlayerHealth _playerHealth;
    private Image _healthBar;

    private void Awake()
    {
        _healthBar = GetComponent<Image>();
    }

    private void Start()
    {
        _playerHealth = GameManager.Instance.player.GetComponent<PlayerHealth>();
        _healthBar.sprite = _healthSprites[0];
    }

    public void UpdateHealthBar()
    {
        _healthBar.sprite = _healthSprites[_playerHealth.StartingHealth - _playerHealth.CurrentHealth];
    }

    public void Death()
    {
        _healthBar.sprite = _deathSprite;
    }
}