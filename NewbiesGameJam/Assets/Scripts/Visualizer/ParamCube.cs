using UnityEngine;

public class ParamCube : MonoBehaviour
{
    [SerializeField] private int _band = 0;
    [SerializeField] private float _startScale = 1f;
    [SerializeField] private float _scaleMultiplier = 10f;

    private void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.BandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
    }
}