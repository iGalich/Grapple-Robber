using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Parameters")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _jumpPower = 20f;
    [SerializeField] private float _adjustableFallJump = 0.5f;
    [SerializeField] private Transform _groundCheckCollider;
    private float _horizontalInput;
    private float _initialMoveSpeed;
    private bool _isInControl;
    private bool _canJump;

    private bool _isGrounded = false;
    private bool _isGrappling = false;
    private bool _isJumping = false;
    private bool _isFalling = false;
    private bool _isJumpingFromWall = false;


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
    //private BoxCollider2D _lastWall; // Used to keep track of the last wall that was jumped off, so you can't wall jump up the same wall

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

    public Vector2 Velocity => _body.velocity;
    public bool IsInControl => _isInControl;
    public float HorizontalInput => _horizontalInput;
    public bool IsJumping => _isJumping;
    public bool IsFalling => _isFalling;
    public bool IsGrabbingWall => _isGrabbingWall;
    public bool IsGrounded => _isGrounded;
    public bool IsGrappling => _isGrappling;

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

        if (!GameManager.Instance.playerHealth.IsDead())
            _isInControl = CheckIfInControl();
        if (!_isInControl)
        {
            _wallJumpCounter -= Time.deltaTime;
        }

        FlipSprite();

        GetInput();

        if (_body.velocity.y <= -0.1f)
        {
            _isFalling = true;
            _isJumping = false;
        }
        else
        {
            _isFalling = false;
        }

        CheckForWall();
        //CheckDash(); disabled for the time being
        CheckJumpRelease(); // Adjustable jump height
    }

    private void FixedUpdate() 
    {
        BasicMovement(_horizontalInput);
        _isGrounded = CheckGround();
        _canJump = false;
        if (_isGrounded)
        {
            _isJumpingFromWall = false;
            //_lastWall = null;
            _coyoteCounter = _coyoteTime; // Reset coyote counter
            _canJump = true;
        }
        else
        {
            if (_coyoteCounter > 0)
                _canJump = true;
            _coyoteCounter -= Time.deltaTime;
        }
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded)
                _coyoteCounter = 0f;
            Jump();
        }
        _isGrappling = Input.GetMouseButton(0);
    }

    private void CheckJumpRelease()
    {
        if (Input.GetKeyUp(KeyCode.Space) && _body.velocity.y > 0)
        {
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * _adjustableFallJump);
        }
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

        if (_canGrabWall && !_isGrounded /*&& _lastWall != _wallHit.collider.GetComponent<BoxCollider2D>()*/)
        {
            // TODO check if this entire line is really necessary
            _isGrabbingWall = (transform.localScale.x > 0 && _horizontalInput > 0) || (transform.localScale.x < 0 && _horizontalInput < 0);
        }

        if (_isGrabbingWall && _isInControl)
        {
            _canJump = true;
            _isJumping = false;
            _isJumpingFromWall = false;
            _body.gravityScale = 0;
            _body.velocity = Vector2.zero;
            _coyoteCounter = _coyoteTime; // Coyote time is reset so player can have a small frame to jump after leaving wall

            // On jump from wall, control is taken away for a short moment
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumpingFromWall = true;
                _isJumping = true;
                _isFalling = false;
                _canJump = false;
                //_lastWall = _wallHit.collider.GetComponent<BoxCollider2D>();
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
        if (_isInControl && _canJump && (_isGrounded || _coyoteCounter >= 0))
        {
            _coyoteCounter = 0f;
            _canJump = false;
            _body.velocity = new Vector2(_body.velocity.x, _jumpPower);
            _isJumping = true;
            _isFalling = false;
        }
    }
    
    private void BasicMovement(float input)
    {
        if (_isInControl)
            _body.velocity = new Vector2(_horizontalInput * _speed, _body.velocity.y);
        else if (!_isJumpingFromWall)
            _body.velocity = new Vector2(0f, _body.velocity.y);
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

    private bool CheckGround()
    {
        _isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheckCollider.position, 0.05f, _groundLayer);
        return colliders.Length > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(_groundCheckCollider.position, 0.05f);
    }

    public void Death()
    {
        _isInControl = false;
        //this.enabled = false;
    }
}