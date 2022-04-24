using UnityEngine;
using System.Collections.Generic;

public class RopeCollider : MonoBehaviour
{
    private GrapplingRope _grapplingRope;
    private List<Vector2> _collisionPoints = new List<Vector2>();
    private PolygonCollider2D _collider;

    private void Start()
    {
        _grapplingRope = GetComponent<GrapplingRope>();
        _collider = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        _collisionPoints = CalculateColliderPoints();
        _collider.SetPath(0, _collisionPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
    }
    
    private List<Vector2> CalculateColliderPoints()
    {
        // Get all positions of line renderer
        Vector3[] positions = _grapplingRope.GetPositions();

        // Get width of line
        float width = _grapplingRope.GetWidth();

        // m = (y2 - y1) / (x2 - x1)
        float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
        float deltaX = (width * 0.5f) * (m / Mathf.Pow(m * m + 1, 0.5f));
        float deltaY = (width * 0.5f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

        // Calculate offset from each point to the collision vertex
        Vector3[] offsets = new Vector3[2];
        offsets[0] = new Vector3(-deltaX, deltaY);
        offsets[1] = new Vector3(deltaX, -deltaY);

        // Generate colliders vertices
        List<Vector2> colliderPositions = new List<Vector2>
        {
            positions[0] + offsets[0],
            positions[1] + offsets[0],
            positions[1] + offsets[1],
            positions[0] + offsets[1]
        };

        return colliderPositions;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_collisionPoints != null)
            _collisionPoints.ForEach(p => Gizmos.DrawSphere(p, 0.1f));
    }
}