using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Events")]
    public UnityEvent OnHealthChange;
    public UnityEvent OnPlayerDeath;

    [Header ("Health")]
    [SerializeField] private int _startingHealth = 6;
    [SerializeField] private int _currentHealth;
    private bool _isDead = false;
    private bool _graceHit = true;

    [Header ("IFrames")]
    [SerializeField] private float _iFramesDuration = 1f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _grappleSprite;
    [SerializeField] private float _iFramesDeltaTime = 0.15f;
    private float _lastHit;
    private WaitForSeconds _iFramesTick;

    [Header ("Grapple Gun")]
    [SerializeField] private GameObject _grappleGunPrefab;
    [SerializeField] private float _pushValue = 1000f;
    private GameObject _grappleGun;

    [Header ("Shake parameters")]
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    [Header ("Particles")]
    [SerializeField] private ParticleSystem _hitParticles;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _healSfx;
    [SerializeField] private AudioClip _damageTaken;

    public int StartingHealth => _startingHealth;
    public int CurrentHealth { get => _currentHealth ; set => _currentHealth = value; }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            _currentHealth = _startingHealth;
    }

    private void Start()
    {
        _iFramesTick = new WaitForSeconds(_iFramesDeltaTime);
        if (SceneManager.GetActiveScene().buildIndex == 0)
            _currentHealth = _startingHealth;
        OnHealthChange.Invoke();
    }

    private void Update()
    {
        //Testing(); // TODO remove
        EmergencyDeath();
    }

    private void EmergencyDeath()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            TakeDamage(10);
    }

    private void Testing()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
            _grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
        }
        if (Input.GetKeyDown(KeyCode.F))
            TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - _lastHit <= _iFramesDuration || damage == 0) return;

        if (_currentHealth == 1 && _graceHit)
        {
            _lastHit = Time.time;
            StartCoroutine(StartIFrames());
            _hitParticles.Play();
            FunctionTimer.Create(() => _hitParticles.Stop(), _hitParticles.main.duration);
            iTween.ShakePosition(_healthBar, Vector3.one * _shakeIntensity, _shakeTime);
            _graceHit = false;
            return;
        }
        _currentHealth -= damage;
        AudioManager.Instance.PlaySound(_damageTaken);

        if (_currentHealth > 0)
        {
            _lastHit = Time.time;
            StartCoroutine(StartIFrames());
            OnHealthChange.Invoke();
            _hitParticles.Play();
            FunctionTimer.Create(() => _hitParticles.Stop(), _hitParticles.main.duration);
            iTween.ShakePosition(_healthBar, Vector3.one * _shakeIntensity, _shakeTime);
        }
        else if (!_isDead)
        {
            OnPlayerDeath.Invoke();
            LaunchGrappleGun();
            FunctionTimer.Create(() => GameManager.Instance.ResetLevel(), 3f);
        }
    }

    private IEnumerator StartIFrames()
    {
        while (Time.time - _lastHit <= _iFramesDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            _grappleSprite.enabled = !_grappleSprite.enabled;
            yield return _iFramesTick;
        }
        _spriteRenderer.enabled = true;
        _grappleSprite.enabled = true;
    }
    
    // Disables and spawns a new grapple gun, which flies off in a random direction
    private void LaunchGrappleGun()
    {
        GameManager.Instance.grappleGun.SetActive(false);
        //GameManager.Instance.player.GetComponent<BoxCollider2D>().enabled = false;
        _grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
        _grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
    }

    public void AddHealth(int value)
    {
        _currentHealth += value;
        AudioManager.Instance.PlaySound(_healSfx);
        if (_currentHealth > _startingHealth)
            _currentHealth = _startingHealth;
        OnHealthChange.Invoke();
        if (_currentHealth > 1)
            _graceHit = true;
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