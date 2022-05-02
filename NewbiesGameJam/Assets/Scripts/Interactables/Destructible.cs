using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _healthPickupPrefab;
    [SerializeField] private float _pushValue = 1000f;
    [SerializeField] private float _dropChance = 50f;
    [SerializeField] private GameObject _particles;
    private GameObject _healthPickup;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rope") || other.CompareTag("Player"))
        {
            Debug.Log(other.name);
            _collider.enabled = false;
            //_grappleGun = Instantiate(_grappleGunPrefab, GameManager.Instance.player.transform.position + new Vector3(0.32f, 0, 0), Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
            //_grappleGun.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
            if (Random.value > (100 - _dropChance) / 100f)
            {
                _healthPickup = Instantiate(_healthPickupPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                _healthPickup.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_pushValue , _pushValue), Random.Range(-_pushValue , _pushValue)));
            }
            Instantiate(_particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}