using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Events")]
    public UnityEvent OnPlayerDeath;

    [Header ("Health")]
    [SerializeField] private int _startingHealth = 6;
    [SerializeField] private int _currentHealth; // serialized for testing purposes
    private bool _isDead = false;

    private void Awake()
    {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(this.name + " took " + damage + " damage");

        _currentHealth -= damage;

        if (_currentHealth > 0)
        {

        }
        else if (!_isDead)
        {
            OnPlayerDeath.Invoke();
        }
    }

    public void AddHealth(int value)
    {
        _currentHealth += value;
    }

    public bool IsFullHealth()
    {
        return _currentHealth == _startingHealth;
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }
}