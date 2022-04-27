using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float _spriteSize = 0.32f;
    
    [Header ("References")]
    private GameObject _player;
    private Transform _target;

    [Header ("Shake parameters")]
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    [Header ("Attack parameters")]
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private Transform _firePoint;
    private float _lastAttack;

    [Header ("Projectile Pool")]
    [SerializeField] private GameObject[] _projectiles;

    [Header ("Animations")]
    private Animator _anim;

    public Transform Target { get => _target; set => _target = value; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = GameManager.Instance.player;
    }

    private void Update()
    {
        FacePlayer();
        AttackPlayer();
    }

    public void GotHit()
    {
        GameManager.Instance.playerAnimator.TriggerKick();
        GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
        TimeManager.Instance.DoSlowmotion(_shakeTime);
        FunctionTimer.Create(() => Death(), _shakeTime * 0.5f);
    }

    private void FacePlayer()
    {
        if (_target == null) return;

        if (_player.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(-_spriteSize, _spriteSize, 0);
        else
            transform.localScale = new Vector3(_spriteSize, _spriteSize);
    }

    private void AttackPlayer()
    {
        if (_target == null)
        {
            _anim.SetBool(ReadyToFireKey, false);
            return;
        }

        _anim.SetBool(ReadyToFireKey, true);
    }

    public void CanFire()
    {
        if (Time.time - _lastAttack < _attackCooldown) return;
        _anim.SetTrigger(FireKey);
    }

    public void Fire()
    {
        int index = FindProjectile();
        _lastAttack = Time.time;
        _projectiles[index].transform.position = _firePoint.position;
        _projectiles[index].SetActive(true);
    }

    private int FindProjectile()
    {
        for (int i = 0; i < _projectiles.Length; i++)
        {
            if (!_projectiles[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private void Death()
    {
        Debug.Log(this + " is dead");
        Destroy(gameObject);
    }

    #region Animation Keys

    private static readonly int ReadyToFireKey = Animator.StringToHash("ReadyToFire");
    private static readonly int FireKey = Animator.StringToHash("Fire");

    #endregion
}