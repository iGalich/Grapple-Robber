using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header ("References")]
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

    [Header ("Movement Parameters")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform _leftBound;
    [SerializeField] private Transform _rightBound;

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
        FacePlayer();
        DecideState();
        SyncHealth();
    }

    private void SyncHealth()
    {
        float healthRatio = (float)_currHealth / (float)_startHealth;
        _healthBarFront.fillAmount = Mathf.Lerp(_healthBarFront.fillAmount, healthRatio, Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _playerObject)
        {
            if (_inAttackAnimation)
            {
                _playerHealth.TakeDamage(_damage);
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
            }
            else if (_playerMovement.IsGrappling)
            {
                GameManager.Instance.playerAnimator.TriggerKick();
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
                TimeManager.Instance.DoSlowmotion(_shakeTime);
                // Player does extra damage when he is low on health
                _currHealth -= _playerHealth.StartingHealth - _playerHealth.CurrentHealth + 1;
                iTween.ShakePosition(_healthBar, Vector3.one * _shakeIntensity, _shakeTime);
            }
            else
            {
                _playerHealth.TakeDamage(_damage);
                GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
            }
        }
    }

    private void DecideState()
    {
        if (Time.time - _lastState > _stateCooldown)
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
        Debug.Log(ability);
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
        _missilePointAnim.SetTrigger(MissileKey);
    }

    private void Slam()
    {
        FacePlayer(true);
        _anim.SetTrigger(SlamKey);
        _lastState = Time.time;
        //Instantiate(_shockwavePrefab, _slamPoint.position, Quaternion.identity);
        FunctionTimer.Create(() => _inAction = false, _stateCooldown);
    }

    private void ActivateShockwave()
    {
        _slamPoint.SetActive(true);
    }

    public void GetMissile()
    {
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
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            yield return null;
        }
        FacePlayer(true);
        _anim.SetBool(WalkKey, false);
        FunctionTimer.Create(() => ChooseAttack(), _stateCooldown);
    }

    private void FacePlayer(bool ignoreBool = false)
    {
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

    #endregion
}