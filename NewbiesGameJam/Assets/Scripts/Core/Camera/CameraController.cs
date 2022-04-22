using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _cameraFollowSpeed = 1f;

    [Header ("Follow on X axis")]
    [SerializeField] private float _aheadDistance = 1f;
    private float _lookAhead;

    [Header ("Follow on Y axis")]
    [SerializeField] private float _upDistance = 1f;
    private float _lookUp;

    [Header ("Shake")]
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _duration = 1f;

    private void Update() 
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        transform.position = new Vector3(_player.position.x + _lookAhead, _player.position.y + _lookUp, transform.position.z);
        _lookAhead = Mathf.Lerp(_lookAhead, _aheadDistance * _player.localScale.x, _cameraFollowSpeed * Time.deltaTime);
        _lookUp = Mathf.Lerp(_lookUp, _upDistance * _player.localScale.y * Mathf.Sign(GameManager.Instance.playerMovement.Velocity.y), _cameraFollowSpeed * Time.deltaTime);
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = _curve.Evaluate(elapsedTime / _duration);
            transform.position = new Vector3(_player.position.x + _lookAhead, _player.position.y + _lookUp, transform.position.z) + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = new Vector3(_player.position.x + _lookAhead, _player.position.y + _lookUp, transform.position.z);
    }
}