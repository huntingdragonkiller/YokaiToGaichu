using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsClimbing = Animator.StringToHash("isClimbing");
    
    [Header("Movement settings")]
    public float speed = 5f;
    public float jumpForce = 7f;
    private bool _isGrounded;
    private bool _onWall;
    private bool _isJumping;
    private float _move;
    private bool _facingRight = true;
    
    public LayerMask wallLayer;
    private float _climbSpeed;
    private float _wallJumpForce;
    private float _jumpHeight;
    
    
    private float _facingDirection = 1f; // 1 for right, -1 for left
    private bool _isTouchingWall;
    
    // Coyote time
    public float wallJumpTime = 0.5f;  // Time window to allow wall jumping
    private float _wallJumpTimer;
    
    private SpriteRenderer _sr;
    private Rigidbody _rb;

    private Animator _anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        _climbSpeed = (3 / 5f * speed);
        _wallJumpForce = speed;
        _jumpHeight = jumpForce;
    }

    void Update()
    {
        // Store movement input
        _move = Input.GetAxis("Horizontal");

        SetupDirectionByRotation();
        
        if (_move != 0)
        {
            _anim.SetBool(IsRunning, true);
        }
        else
        {
            _anim.SetBool(IsRunning, false);
        }
        
        _anim.SetBool(IsJumping, _isJumping);
        _anim.SetBool(IsClimbing, _onWall);

        // Horizontal movement
        _rb.linearVelocity = new Vector3(_move * speed, _rb.linearVelocity.y, 0);
        
        // Jumping
        if (Input.GetButtonDown("Jump") && (_isGrounded || _onWall))
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, 0);
            _isGrounded = false;
            _onWall = false;
        }

        if (_isGrounded || _onWall)
        {
            _isJumping = false;
        }
        else {_isJumping = true;}

        if (!_onWall)
        {
            _sr.flipY = false;
        }
        
        // Climbing
        _isTouchingWall = IsTouchingWall();
        if (_isTouchingWall)
        {
            _wallJumpTimer = wallJumpTime;
            if (Input.GetKey(KeyCode.W)) // Climb up with W
            {
                WallClimb(1f); // Move Up
                _sr.flipY = false;
            }
            else if (Input.GetKey(KeyCode.S)) // Climb down with S
            {
                WallClimb(-1f); // Move down
                _sr.flipY = true;
            }
            else
            {
                // If no input, stop vertical movement
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, 0);
            }
        }
        else
        {
            _rb.useGravity = true;
            _wallJumpTimer -= Time.deltaTime;
            if(_wallJumpTimer <= 0)
                _wallJumpTimer = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && (_wallJumpTimer > 0))
        {
            Debug.Log("Space is pressed in the air");
            WallJump();
        }
        
        // Handle wall flip direction
        if (Input.GetAxis("Horizontal") > 0) // Moving right
        {
            _facingDirection = 1f;
        }
        else if (Input.GetAxis("Horizontal") < 0) // Moving left
        {
            _facingDirection = -1f;
        }

        bool IsTouchingWall()
        {
            float wallCheckDistance = 0.8f;
            Vector3 direction = new Vector3(_facingDirection, 0, 0);
            // Dray the ray in scene view
            Debug.DrawRay(transform.position, direction * wallCheckDistance, Color.red);
            return Physics.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        }

        void WallClimb(float direction)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, direction * _climbSpeed, 0);
            _rb.useGravity = false;
        }

        void WallJump()
        {
            Vector3 jumpDirection = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
            _rb.linearVelocity = new Vector3(jumpDirection.x * _wallJumpForce, _jumpHeight, 0);
        }
        
        Debug.Log(_wallJumpTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            _onWall = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            _onWall = false;
        }
    }

    private void SetupDirectionByRotation()
    {
        if (_move < 0 && _facingRight || _move > 0 && !_facingRight)
        {
            _facingRight = !_facingRight;
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}
