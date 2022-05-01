using UnityEngine;

public class ShockwaveStart : MonoBehaviour
{
    [SerializeField] private GameObject _shockwaveRight;
    [SerializeField] private GameObject _shockwaveLeft;

    private void SpawnShockwaves()
    {
        Instantiate(_shockwaveRight, transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity);
        Instantiate(_shockwaveLeft, transform.position + new Vector3(-1f, 0f, 0f), Quaternion.identity);
        this.gameObject.SetActive(false);
    }
}