using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts References")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int[] grappableLayerNumbers;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask[] _grappableLayers; // when adding a new layer to the array, add the layer index to the int array as well

    [Header("Main Camera")]
    public Camera m_camera;

    [Header("Transform Ref")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;
    public Vector2 direction;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _launchRopeSfx;
    [SerializeField] private AudioClip _impactSfx;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

        if (_grappableLayers.Length != grappableLayerNumbers.Length)
            Debug.LogError("Please check that all grappleable layers are in layer number array and vice versa");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    //gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                    gunHolder.position = Vector2.MoveTowards(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                    //gunHolder.position = Vector2.LerpUnclamped(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            //m_rigidbody.gravityScale = 1;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        // if (Physics2D.Raycast(firePoint.position, distanceVector.normalized, _grappableLayer, ~_playerLayer))
        // {
        //     RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, _grappableLayer, ~_playerLayer);
        //     if (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
        //     {
        //         if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
        //         {
        //             grapplePoint = _hit.point;
        //             grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
        //             grappleRope.enabled = true;
        //         }
        //     }
        // }

        for (int i = 0; i < _grappableLayers.Length; i++)
        {
            if (Physics2D.Raycast(firePoint.position, distanceVector.normalized, _grappableLayers[i], ~_playerLayer))
            {
                RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, _grappableLayers[i], ~_playerLayer);
                for (int j = 0; j < grappableLayerNumbers.Length; j++)
                {
                    if (_hit.transform.gameObject.layer == grappableLayerNumbers[j] || grappleToAll)
                    {
                        if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                        {
                            grapplePoint = _hit.point;
                            grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                            grappleRope.enabled = true;
                            AudioManager.Instance.PlaySound(_launchRopeSfx);
                            break;
                        }
                    }
                }
            }
        }
    }

    public bool IsRopeOn()
    {
        return grappleRope.enabled; 
    }

    public void Grapple()
    {
        AudioManager.Instance.PlaySound(_impactSfx);
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;
                    direction = distanceVector;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

}