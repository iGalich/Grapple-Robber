// using UnityEngine;
// using System.Collections;

// public class CameraShake : MonoBehaviour
// {
//     [SerializeField] private float _duration = 1f;
//     [SerializeField] private bool _start = false; // testing shake
//     [SerializeField] private AnimationCurve _curve;
//     private CameraController _cameraController;

//     private void Awake()
//     {
//         _cameraController = GetComponent<CameraController>();
//     }

//     private void Update()
//     {
//         if (_start)
//         {
//             _start = false;
//             StartCoroutine(Shake());
//         }
//     }

//     public void StartShake()
//     {
//         StartCoroutine(Shake());
//     }

//     private IEnumerator Shake()
//     {
//         Vector3 startPosition = transform.position;
//         float elapsedTime = 0f;

//         while (elapsedTime < _duration)
//         {
//             elapsedTime += Time.deltaTime;
//             float strength = _curve.Evaluate(elapsedTime / _duration);
//             transform.position = startPosition + Random.insideUnitSphere * strength;
//             yield return null;
//         }

//         //transform.position = startPosition;
//         transform.position = new Vector3(_cameraController.Player.position.x + _cameraController.LookAhaed, transform.position.y, transform.position.z);
//     }
// }