using UnityEngine;
using Cinemachine;

public class CinemachineOffset : MonoBehaviour
{
    private float _camSpeed = 0.2f;
    private Rigidbody2D _playerBody;
    private Transform _player;
    private CinemachineVirtualCamera _vCam;
    private CinemachineFramingTransposer _camTransposer;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _camTransposer = _vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        _player = GameManager.Instance.player.transform;
        _playerBody = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float offsetX = _player.localScale.x;
        float offsetY = (_playerBody.velocity.y >= 0f) ? 1 : -1;

        _camTransposer.m_TrackedObjectOffset = Vector3.Lerp(_camTransposer.m_TrackedObjectOffset, new Vector3(offsetX, offsetY, 0f), _camSpeed * Time.deltaTime);
        //_camTransposer.m_TrackedObjectOffset = new Vector3(offsetX, offsetY, 0f);
    }
}