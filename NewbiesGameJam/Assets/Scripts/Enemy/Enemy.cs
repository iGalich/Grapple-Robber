using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private float _spriteSize = 0.32f;
    
    [Header ("References")]
    private GameObject _player;
    private Transform _target;

    [Header ("Shake parameters")]
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    [Header ("Attack parameters")]
    [SerializeField] private int _meleeDamage = 1;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private bool _dummy = false;
    private float _lastAttack;
    private bool _isDead = false;

    [Header ("Projectile Pool")]
    [SerializeField] private GameObject[] _projectiles;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _fireSfx;
    [SerializeField] private AudioClip _deathSfx;

    [Header ("Animations")]
    [SerializeField] private float _fadeSpeed = 0.5f;
    private Animator _anim;

    public Transform Target { get => _target; set => _target = value; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = GameManager.Instance.player;
        if (transform.localScale.y < 0)
            _spriteSize *= -1;
    }

    private void Update()
    {
        if (_dummy || _isDead) return;
        
        FacePlayer();
        AttackPlayer();
    }

    public void GotHit()
    {
        if (_isDead) return;
        GameManager.Instance.playerAnimator.TriggerKick();
        GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
        AudioManager.Instance.PlaySound(_deathSfx);
        //TimeManager.Instance.DoSlowmotion(_shakeTime);
        Death();

        //FunctionTimer.Create(() => Death(), _shakeTime * 0.5f);
    }

    public void OnRopeHit()
    {
        GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
        Death();
    }

    private void FacePlayer()
    {
        if (_target == null) return;

        if (_player.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(-_spriteSize, _spriteSize, 0);
        else
            transform.localScale = new Vector3(_spriteSize, _spriteSize);
        
        if (_spriteSize < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 0f);
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
        AudioManager.Instance.PlaySound(_fireSfx);
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
        _isDead = true;
        // Debug.Log(this + " is dead");
        // Destroy(gameObject);
        _anim.SetTrigger(DeathKey);
    }

    private void StartFadeAway()
    {
        StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        while (GetComponent<Renderer>().material.color.a > 0)
        {
            Color objectColor = GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a - _fadeSpeed * Time.deltaTime;
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            GetComponent<Renderer>().material.color = objectColor;
            yield return null;
        }
        Destroy(gameObject);
    }

    #region Animation Keys

    private static readonly int ReadyToFireKey = Animator.StringToHash("ReadyToFire");
    private static readonly int FireKey = Animator.StringToHash("Fire");
    private static readonly int DeathKey = Animator.StringToHash("Death");

    #endregion
}