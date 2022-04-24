using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _healthPickupPrefab;
    [SerializeField] private float _pushValue = 1000f;
    [SerializeField] private float _dropChance = 50f;
    private GameObject _healthPickup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rope") || other.CompareTag("Player"))
        {
            //_grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
            //_grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
            if (Random.value > (100 - _dropChance) / 100f)
            {
                _healthPickup = Instantiate(_healthPickupPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                _healthPickup.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
            }

            Destroy(gameObject);
        }
    }
}