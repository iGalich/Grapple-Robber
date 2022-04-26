using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private float _kickToFallTime = 0.25f;
    private Animator _anim;
    private PlayerMovement _player;
    private bool _checkingAnimations;

    public bool SetCheckingAnimation { get => _checkingAnimations; set => _checkingAnimations = value; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = GameManager.Instance.playerMovement;
    }

    private void Update()
    {
        if (_player == null) return;

        if (_checkingAnimations)
            UpdatePlayerAnimation();
        else
            _checkingAnimations = _player.IsGrounded;
    }

    private void UpdatePlayerAnimation()
    {
        _anim.SetBool(KickKey, false);
        _anim.SetBool(HurtKey, false);
        _anim.SetBool(RunKey, _player.HorizontalInput != 0);
        _anim.SetBool(FallKey, _player.IsFalling);
        _anim.SetBool(JumpKey, _player.IsJumping);
        _anim.SetBool(GroundedKey, _player.IsGrounded);
        _anim.SetBool(WallGrabKey, _player.IsGrabbingWall);
    }

    public void TriggerKick()
    {
        _checkingAnimations = false;
        _anim.SetBool(KickKey, true);
        _anim.SetBool(JumpKey, false);
        _anim.SetBool(FallKey, false);

        //switch over to fall animation
        FunctionTimer.Create(() => _anim.SetBool(KickKey, false), _kickToFallTime);
        FunctionTimer.Create(() => _anim.SetBool(FallKey, true), _kickToFallTime);
    }

    public void TriggerHurt()
    {
        _checkingAnimations = false;
        _anim.SetBool(JumpKey, false);
        _anim.SetBool(FallKey, false);
        _anim.SetBool(HurtKey, true);

        FunctionTimer.Create(() => _checkingAnimations = true, _anim.GetCurrentAnimatorStateInfo(0).length + _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void Death()
    {
        _checkingAnimations = false;
        _anim.SetBool(JumpKey, false);
        _anim.SetBool(FallKey, false);
        _anim.SetTrigger(DeathKey);
    }

    #region Animation Keys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int RunKey = Animator.StringToHash("Running");
    private static readonly int FallKey = Animator.StringToHash("Falling");
    private static readonly int JumpKey = Animator.StringToHash("Jumping");
    private static readonly int WallGrabKey = Animator.StringToHash("GrabbingWall");
    private static readonly int KickKey = Animator.StringToHash("Kick");
    private static readonly int HurtKey = Animator.StringToHash("Hurt");
    private static readonly int DeathKey = Animator.StringToHash("Death");

    #endregion
}