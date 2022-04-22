using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Events")]
    public UnityEvent OnHealthChange;
    public UnityEvent OnPlayerDeath;

    [Header ("Health")]
    [SerializeField] private int _startingHealth = 6;
    [SerializeField] private int _currentHealth; // serialized for testing purposes
    private bool _isDead = false;

    [Header ("Grapple Gun")]
    [SerializeField] private GameObject _grappleGunPrefab;
    [SerializeField] private float _pushValue = 1000f;
    private GameObject _grappleGun;

    public int StartingHealth => _startingHealth;
    public int CurrentHealth => _currentHealth;

    private void Awake()
    {
        _currentHealth = _startingHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Testing
        {
            _grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
            _grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(this.name + " took " + damage + " damage");

        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            OnHealthChange.Invoke();
        }
        else if (!_isDead)
        {
            OnPlayerDeath.Invoke();
            LaunchGrappleGun();
        }
    }
    
    // Disables and spawns a new grapple gun, which flies off in a random direction
    private void LaunchGrappleGun()
    {
        GameManager.Instance.grappleGun.SetActive(false);
        GameManager.Instance.player.GetComponent<BoxCollider2D>().enabled = false;
        _grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
        _grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
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