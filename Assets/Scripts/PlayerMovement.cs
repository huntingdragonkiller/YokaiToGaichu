using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    private bool _isGrounded;
    private float _move;
    
    public LayerMask wallLayer;
    public float climbSpeed = 3f;
    public float wallJumpForce = 5f;
    public float jumpHeight = 7f;
    public float wallJumpTime = 0.5f;  // Time window to allow wall jumping
    private bool canWallJump = false;
    private float lastWallTouchTime;
    private float facingDirection = 1f; // 1 for right, -1 for left
    private bool _isTouchingWall;
    
    private Rigidbody _rb;
    private SpriteRenderer _sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Store movement input
        _move = Input.GetAxis("Horizontal");

        if (_move > 0)
            _sr.flipX = false; // Facing right
        else if (_move < 0)
            _sr.flipX = true; // Facing left
        
        // Jumping
    if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, 0);
            _isGrounded = false;
        }
    }
    void FixedUpdate()
    {
        // Horizontal movement
        _rb.linearVelocity = new Vector3(_move * speed, _rb.linearVelocity.y, 0);
        _isTouchingWall = IsTouchingWall();

        if (_isTouchingWall)
        {
            canWallJump = true;
            lastWallTouchTime = 0.25f;
            if (Input.GetKey(KeyCode.W)) // Climb up with W
            {
                WallClimb(1f); // Move Up
            }
            else if (Input.GetKey(KeyCode.S)) // Climb down with S
            {
                WallClimb(-1f); // Move down
            }
            else
            {
                // If no input, stop vertical movement
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.Space) && (canWallJump))
            {
                WallJump();
            }
        }
        else
        {
            _rb.useGravity = true;
            lastWallTouchTime -= Time.deltaTime;
        }
        

        // Handle wall flip direction
        if (Input.GetAxis("Horizontal") > 0) // Moving right
        {
            facingDirection = 1f;
            _sr.flipX = false;
        }
        else if (Input.GetAxis("Horizontal") < 0) // Moving left
        {
            facingDirection = -1f;
            _sr.flipX = true;

        }

        bool IsTouchingWall()
        {
            float wallCheckDistance = 0.5f;
            Vector3 direction = new Vector3(facingDirection, 0, 0);
            // Dray the ray in scene view
            Debug.DrawRay(transform.position, direction * wallCheckDistance, Color.red);
            return Physics.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        }

        void WallClimb(float direction)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, direction * climbSpeed, 0);
            _rb.useGravity = false;
        }

        void WallJump()
        {
            Vector3 jumpDirection = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
            _rb.linearVelocity = new Vector3(jumpDirection.x * wallJumpForce, jumpHeight, 0);
            canWallJump = false; // Reset the ability to wall jump
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
}
