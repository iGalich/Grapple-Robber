using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header ("References")]
    private GameObject _player;

    [Header ("Shake parameters")]
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    private void Start()
    {
        _player = GameManager.Instance.player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            GameManager.Instance.playerAnimator.TriggerKick();
            //GameManager.Instance.cameraController.CameraShake.StartShake();
            GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
            TimeManager.Instance.DoSlowmotion(_shakeTime);
            //Death();
        }
    }

    private void Death()
    {
        Debug.Log(this + " is dead");
        Destroy(gameObject);
    }
}