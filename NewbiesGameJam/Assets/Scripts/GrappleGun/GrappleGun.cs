using UnityEngine;
using System.Collections;

public class GrappleGun : MonoBehaviour
{
    [SerializeField] private LineRenderer _rope;
    [SerializeField] private LayerMask _grappableLayer; // I don't think grappable is a real word
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _grappleSpeed = 10f;
    [SerializeField] private float _grappleFireSpeed = 10f;
    private bool _isGrappling = false;
    private bool _isRetracting = false;
    private Vector2 _target;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isGrappling)
        {
            _isGrappling = true;
        }
    }

    private void FixedUpdate()
    {
        if (_isGrappling)
            StartGrapple();

        if (_isRetracting)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector2 grapplePosition = Vector2.Lerp(GameManager.Instance.player.position, _target, _grappleSpeed * Time.deltaTime);
        GameManager.Instance.player.position = grapplePosition;

        _rope.SetPosition(0, GameManager.Instance.player.position);

        if (Vector2.Distance(GameManager.Instance.player.position, _target) < 0.5f)
        {
            _isRetracting = false;
            _isGrappling = false;
            _rope.enabled = false;
        }
    }

    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - GameManager.Instance.player.position;
        RaycastHit2D hit = Physics2D.Raycast(GameManager.Instance.player.position, direction, _maxDistance, _grappableLayer);

        if (hit.collider != null)
        {
            _isGrappling = true;
            _target = hit.point;
            _rope.enabled = true;
            _rope.positionCount = 2;
            
            StartCoroutine(Grapple());
        }
    }

    private IEnumerator Grapple()
    {
        var time = 1;
        _rope.SetPosition(0, GameManager.Instance.player.position);
        _rope.SetPosition(1, GameManager.Instance.player.position);

        Vector2 newPosition;

        for (float t = 0; t < time; t += _grappleFireSpeed * Time.deltaTime)
        {
            newPosition = Vector2.Lerp(GameManager.Instance.player.position, _target, t / time);
            _rope.SetPosition(0, GameManager.Instance.player.position);
            _rope.SetPosition(1, newPosition);
            yield return null;
        }

        _rope.SetPosition(1, _target);
        _isRetracting = true;
    }
}