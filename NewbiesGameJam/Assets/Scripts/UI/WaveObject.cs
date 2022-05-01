using UnityEngine;

public class WaveObject : MonoBehaviour
{
    [SerializeField] private float _waveModifier = 0.06f;

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * _waveModifier, 0);
    }
}