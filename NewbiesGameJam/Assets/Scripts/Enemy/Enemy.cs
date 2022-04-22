using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _player;

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
            GameManager.Instance.cameraController.ShakeCamera();
            TimeManager.Instance.DoSlowmotion();
            //Death();
        }
    }

    private void Death()
    {
        Debug.Log(this + " is dead");
        Destroy(gameObject);
    }
}