using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private PlayerMovement _playerMovement;
    private GameObject _player;

    private void Awake()
    {
        _playerMovement = GameManager.Instance.playerMovement;
        _player = GameManager.Instance.player;
    }

    private void Update()
    {
        if (_player == null)
            return;

        // if (_playerMovement.IsRunning)
        //     _anim.SetBool();
    }
}