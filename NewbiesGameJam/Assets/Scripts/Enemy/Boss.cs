using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private GameObject _finalDisk;
    private float _spriteSize;
    private SpriteRenderer _sprite;
    private GameObject _playerObject;
    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;
    private Transform _player;
    private Animator _anim;

    [Header ("Health")]
    [SerializeField] private int _startHealth = 100;
    [SerializeField] private int _currHealth;
    [SerializeField] private Image _healthBarFront;
    [SerializeField] private GameObject _healthBar;
    private bool _isAlive = true;

    [Header ("Movement Parameters")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform _leftBound;
    [SerializeField] private Transform _rightBound;
    private bool _canMove;

    [Header ("Shake Parameters")]
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    [Header ("State Parameters")]
    [SerializeField] private float _stateCooldown = 3f;
    [SerializeField] private int _damage = 1;
    private float _lastState = Mathf.NegativeInfinity;
    private bool _inAction = false;
    private int _lastAbility = -1;
    private bool _inAttackAnimation = false;

    [Header ("Missile")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject[] _missiles;
    [SerializeField] private Animator _missilePointAnim;

    [Header ("Slash")]
    [SerializeField] private BoxCollider2D _slashCollider;

    [Header ("Slam")]
    [SerializeField] private GameObject _slamPoint;

    [Header ("Final screen")]
    [SerializeField] private CanvasGroup _finalScreen;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _damageTaken;
    [SerializeField] private AudioClip _slamSfx;
    [SerializeField] private AudioClip _bossScreamSfx;

    public bool CanMove { get => _canMove; set => _canMove = value; }

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _spriteSize = transform.localScale.x;
        _player = GameManager.Instance.player.transform;
        _playerObject = GameManager.Instance.player;
        _playerMovement = GameManager.Instance.playerMovement;
        _playerHealth = GameManager.Instance.playerHealth;
        _currHealth = _startHealth;
    }

    private void Update()
    {
        if (!_canMove) return;
        FacePlayer();
        DecideState();
        SyncHealth();
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (_currHealth <= 0 && _isAlive)
        {
            _playerHealth.CurrentHealth += 5;
            _inAction = false;
            _isAlive = false;
            _anim.SetTrigger(DeathKey);
            DeathScream();
            FunctionTimer.Create(() => _healthBar.SetActive(false), 4.5f);
            FunctionTimer.Create(() => AudioManager.Instance.MusicSource.Stop(), 4.5f);
            FunctionTimer.Create(() => _finalDisk.SetActive(true), 5f); 
        }
    }

    private void DeathScream()
    {
        AudioManager.Instance.PlaySound(_bossScreamSfx);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_damageTaken), 1f);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_damageTaken), 2f);
        FunctionTimer.Create(() => AudioManager.Instance.PlaySound(_damageTaken), 3f);
    }

    public void WinScreenCo()
    {
        StartCoroutine(ShowWinScreen());
    }

    private IEnumerator ShowWinScreen()
    {
        //_playerMovement.ForceMovementStop = false;
        while (_finalScreen.alpha < 1)
        {
            _finalScreen.alpha += 0.0005f;
            yield return null;
        }
        _finalScreen.alpha = 1;
    }

    private void SyncHealth()
    {
        float healthRatio = (float)_currHealth / (float)_startHealth;
        _healthBarFront.fillAmount = Mathf.Lerp(_healthBarFront.fillAmount, healthRatio, Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _playerObject && _isAlive)
        {
            if (_inAttackAnimation)
            {
                _playerHealth.TakeDamage(_damage);
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
            }
            else if (_playerMovement.IsGrappling)
            {
                AudioManager.Instance.PlaySound(_damageTaken);
                GameManager.Instance.playerAnimator.TriggerKick();
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
                TimeManager.Instance.DoSlowmotion(_shakeTime);
                // Player does extra damage when he is low on health
                _currHealth -= _playerHealth.StartingHealth - _playerHealth.CurrentHealth + 1;
                iTween.ShakePosition(_healthBar, Vector3.one * _shakeIntensity * 5f, _shakeTime);
            }
            else
            {
                _playerHealth.TakeDamage(_damage);
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
            }
        }
    }

    public void PlaySlamSfx()
    {
        AudioManager.Instance.PlaySound(_slamSfx);
    }

    private void DecideState()
    {
        if (Time.time - _lastState > _stateCooldown && _isAlive)
        {
            GotoPlayer();
        }
    }

    private void ChooseAttack()
    {
        int ability;
        do
        {
            ability = Random.Range(0, 3);
        } while (ability == _lastAbility);
        _lastAbility = ability;
        
        switch (ability)
        {
            case 0:
                Slash();
                break;
            case 1:
                Missile();
                break;
            case 2:
                Slam();
                break;
        }
    }

    private void Slash()
    {
        if (!_isAlive) return;
        FacePlayer(true);
        _anim.SetTrigger(SlashKey);
        _lastState = Time.time;
        FunctionTimer.Create(() => _inAction = false, _stateCooldown);
    }

    private void AttackAnimation()
    {
        _inAttackAnimation = !_inAttackAnimation;
    }

    private void Missile()
    {
        if (!_isAlive) return;
        _missilePointAnim.SetTrigger(MissileKey);
    }

    private void Slam()
    {
        if (!_isAlive) return;
        FacePlayer(true);
        _anim.SetTrigger(SlamKey);
        _lastState = Time.time;
        //Instantiate(_shockwavePrefab, _slamPoint.position, Quaternion.identity);
        FunctionTimer.Create(() => _inAction = false, _stateCooldown);
    }

    private void ActivateShockwave()
    {
        if (!_isAlive) return;
        _slamPoint.SetActive(true);
    }

    public void GetMissile()
    {
        if (!_isAlive) return;
        int index = FindProjectile();
        _lastState = Time.time;
        _missiles[index].transform.position = _firePoint.position;
        _missiles[index].SetActive(true);
        FunctionTimer.Create(() => _inAction = false, _stateCooldown);
    }

    private int FindProjectile()
    {
        for (int i = 0; i < _missiles.Length; i++)
        {
            if (!_missiles[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private void SwitchSlashHitBox()
    {
        _slashCollider.enabled = !_slashCollider.enabled;
    }

    private void GotoPlayer()
    {
        if (_inAction) return;

        _inAction = true;
        _anim.SetBool(WalkKey, true);
        StartCoroutine(MoveCo());
    }
    
    private IEnumerator MoveCo()
    {
        float xValue;
        if (_player.position.x >= _leftBound.position.x && _player.position.x <= _rightBound.position.x)
            xValue = _player.position.x;
        else if (_player.position.x < _leftBound.position.x)
            xValue = _leftBound.position.x;
        else
            xValue = _rightBound.position.x;

        Vector3 targetPosition = new Vector3(xValue, transform.position.y, transform.position.z);
        while (transform.position != targetPosition && _isAlive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            yield return null;
        }
        FacePlayer(true);
        _anim.SetBool(WalkKey, false);
        FunctionTimer.Create(() => ChooseAttack(), _stateCooldown);
    }

    public void FacePlayer(bool ignoreBool = false)
    {
        if (!_isAlive) return;
        
        if (!ignoreBool)
            if (_inAction) 
                return;

        if (_player.position.x < transform.position.x)
            transform.localScale = new Vector3(-_spriteSize, _spriteSize, 0);
        else
            transform.localScale = new Vector3(_spriteSize, _spriteSize);
    }

    #region Animation Keys

    private static readonly int WalkKey = Animator.StringToHash("Walking");
    private static readonly int SlashKey = Animator.StringToHash("Slashing");
    private static readonly int MissileKey = Animator.StringToHash("Missile");
    private static readonly int SlamKey = Animator.StringToHash("Slamming");
    private static readonly int DeathKey = Animator.StringToHash("Death");

    #endregion
}