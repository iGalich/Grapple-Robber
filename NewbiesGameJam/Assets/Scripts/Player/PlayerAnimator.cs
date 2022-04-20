using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _anim;
    private PlayerMovement _player;

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

        UpdatePlayerAnimation();
    }

    private void UpdatePlayerAnimation()
    {
        _anim.SetBool(RunKey, _player.HorizontalInput != 0);
        _anim.SetBool(FallKey, _player.IsFalling);
        _anim.SetBool(JumpKey, _player.IsJumping);
        _anim.SetBool(GroundedKey, _player.IsGrounded());
        _anim.SetBool(WallGrabKey, _player.IsGrabbingWall);
    }

    #region Animation Leys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int RunKey = Animator.StringToHash("Running");
    private static readonly int FallKey = Animator.StringToHash("Falling");
    private static readonly int JumpKey = Animator.StringToHash("Jumping");
    private static readonly int WallGrabKey = Animator.StringToHash("GrabbingWall");

    #endregion
}