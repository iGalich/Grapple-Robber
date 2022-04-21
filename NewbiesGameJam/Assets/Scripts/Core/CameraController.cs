using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool _camFollowRoom = true; // Otherwise follows ahead of player

    [Header ("Follow Room")]
    [SerializeField] private float _speed = 0.5f;
    private float _currPosX;
    private Vector3 _velocity = Vector3.zero;

    [Header ("Follow Player")]
    [SerializeField] private float _aheadDistance = 1f;
    [SerializeField] private float _cameraFollowSpeed = 1f;
    [SerializeField] private Transform _player; // player to follow
    private float _lookAhead;

    private void Update() 
    {
        if (_camFollowRoom)
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_currPosX, transform.position.y, transform.position.z), ref _velocity, _speed);
        else // Follow ahead of player
        {
            transform.position = new Vector3(_player.position.x + _lookAhead, transform.position.y, transform.position.z);
            _lookAhead = Mathf.Lerp(_lookAhead, _aheadDistance * _player.localScale.x, _cameraFollowSpeed * Time.deltaTime);
        }
    }

    public void MoveToNewRoom(Transform newRoom)
    {
        _currPosX = newRoom.position.x;
    }
}