using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Parameters")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _jumpPower = 20f;
    private float _horizontalInput;
    private float _initialMoveSpeed;
    private bool _isInControl;
    private bool _canJump;
    private bool _isGrounded; // used for debugging only
    private bool _isGrappling = false;

    [Header ("Coyote Time")]
    [SerializeField] private float _coyoteTime = 0.25f;
    private float _coyoteCounter;

    [Header ("Wall Jump")]
    [SerializeField] private float _wallDistance = 0.55f;
    [SerializeField] private float _wallJumpTime = 0.2f;
    private float _wallJumpCounter = Mathf.NegativeInfinity;
    private float _initialGravity;
    private bool _canGrabWall;
    private bool _isGrabbingWall;
    private RaycastHit2D _wallHit;
    private BoxCollider2D _lastWall; // Used to keep track of the last wall that was jumped off, so you can't wall jump up the same wall

    [Header ("Dash")]
    [SerializeField] private float _dashPower = 2f;
    [SerializeField] private float _dashLength = 0.5f;
    [SerializeField] private float _dashCooldown = 1f;
    private float _dashCounter;
    private float _dashCooldownCounter;

    [Header ("Layers")]
    [SerializeField] private LayerMask _groundLayer;

    // References
    private BoxCollider2D _collider;
    private Rigidbody2D _body;

    public bool IsInControl => _isInControl;

    private void Awake() 
    {
        _collider = GetComponent<BoxCollider2D>();
        _body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _initialGravity = _body.gravityScale;
        _initialMoveSpeed = _speed;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");

        _isInControl = CheckIfInControl();
        if (!_isInControl)
        {
            _wallJumpCounter -= Time.deltaTime;
        }

        FlipSprite();

        GetInput();

        CheckForWall();
        CheckDash();
        CheckJumpRelease(); // Adjustable jump height
    }

    private void FixedUpdate() 
    {
        BasicMovement(_horizontalInput);

        if (IsGrounded())
        {
            _isGrounded = true;
            _lastWall = null;
            _coyoteCounter = _coyoteTime; // Reset coyote counter
            _canJump = true;
        }
        else
        {
            _isGrounded = false;
            _coyoteCounter -= Time.deltaTime;
        }
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        _isGrappling = Input.GetMouseButton(0);
    }
    
    private void CheckJumpRelease()
    {
        if (Input.GetKeyUp(KeyCode.Space) && _body.velocity.y > 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * 0.5f);
    }

    private void CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownCounter <= 0 && _dashCounter <= 0)
        {
            _speed *= _dashPower;
            _dashCounter = _dashLength;
        }

        if (_dashCounter > 0)
        {
            _dashCounter -= Time.deltaTime;
            if (_dashCounter < 0)
            {
                _speed = _initialMoveSpeed;
                _dashCooldownCounter = _dashCooldown;
            }
        }

        if (_dashCooldownCounter > 0)
        {
            _dashCooldownCounter -= Time.deltaTime;
        }
    }

    private bool CheckIfInControl()
    {
        return _wallJumpCounter <= 0;
    }

    private void CheckForWall()
    {
        _canGrabWall = IsOnWall();
        _isGrabbingWall = false;

        if (_canGrabWall && !IsGrounded() && _lastWall != _wallHit.collider.GetComponent<BoxCollider2D>())
        {
            // TODO check if this entire line is really necessary
            _isGrabbingWall = (transform.localScale.x > 0 && _horizontalInput > 0) || (transform.localScale.x < 0 && _horizontalInput < 0);
        }

        if (_isGrabbingWall && _isInControl)
        {
            _canJump = true;
            _body.gravityScale = 0;
            _body.velocity = Vector2.zero;
            _coyoteCounter = _coyoteTime; // Coyote time is reset so player can have a small frame to jump after leaving wall

            // On jump from wall, control is taken away for a short moment
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _canJump = false;
                _lastWall = _wallHit.collider.GetComponent<BoxCollider2D>();
                _wallJumpCounter = _wallJumpTime;
                _isInControl = false;
                _body.gravityScale = _initialGravity;
                _body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * _speed, _jumpPower); // send the player away from the wall
                _isGrabbingWall = false;
            }
        }
        else if (!_isGrappling)
        {
            _body.gravityScale = _initialGravity;
        }
    }

    private void Jump()
    {
        if (_isInControl && _canJump && (IsGrounded() || _coyoteCounter >= 0))
        {
            _body.velocity = new Vector2(_body.velocity.x, _jumpPower);
            _canJump = false;
        }
    }
    
    private void BasicMovement(float input)
    {
        if (_isInControl)
            _body.velocity = new Vector2(_horizontalInput * _speed, _body.velocity.y);
    }

    private void FlipSprite()
    {
        if (_body.velocity.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (_body.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private bool IsOnWall()
    {
        _wallHit = Physics2D.Raycast(transform.position, new Vector2(_wallDistance * Mathf.Sign(transform.localScale.x), 0), _wallDistance, _groundLayer);
        Debug.DrawRay(transform.position, new Vector2(_wallDistance, 0) * Mathf.Sign(transform.localScale.x), Color.red);
        return _wallHit;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0 , Vector2.down, 0.1f, _groundLayer);
        
        return raycastHit.collider != null;
    }
}