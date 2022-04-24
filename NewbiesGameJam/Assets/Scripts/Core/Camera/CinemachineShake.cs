using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineVirtualCamera _cmVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _cmBasicMultiChannelPerlin;
    private float _shakeTimer;
    private float _startingIntensity;
    private float _shakeTimerTotal;

    private void Awake()
    {
        _cmVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cmBasicMultiChannelPerlin = _cmVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.unscaledDeltaTime;
            _cmBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, 1 - (_shakeTimer / _shakeTimerTotal));
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        _cmBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;
        _startingIntensity = intensity;
        _shakeTimerTotal = time;
    }
}