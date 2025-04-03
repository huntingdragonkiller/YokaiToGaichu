using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsClimbing = Animator.StringToHash("isClimbing");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int IsIdle = Animator.StringToHash("isIdle");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");

    [Header("Movement settings")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    private bool _isGrounded;
    [HideInInspector]
    public bool onWall;
    private bool _isJumping;
    private float _move;
    private float _moveY;
    private bool _facingRight = true;
    
    [Header("Jump settings")]
    public float climbSpeed;
    private float _wallJumpForce;
    public float jumpHeight;
    public LayerMask wallLayer;
    // Coyote time
    public float wallJumpTime = 0.5f;  // Time window to allow wall jumping
    private float _wallJumpTimer;
    private bool _isTouchingWall;
    
    private bool _isAttacking;
    private float _fallingThreshold = -1f;
    private bool _isFalling;

    [Header("Audio")] 
    public AudioClip[] footSteps;
    public AudioClip jumpSound;
    
    private float _facingDirection = 1f; // 1 for right, -1 for left
    private SpriteRenderer _sr;
    private Rigidbody _rb;
    private Animator _anim;
    private PlayerAttack _playerAttack;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerAttack = GameObject.Find("Attack").GetComponent<PlayerAttack>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        _wallJumpForce = speed;
    }

    void Update()
    {
        _isAttacking = _playerAttack.isAttacking;
        // Store movement input
        _move = Input.GetAxis("Horizontal");
        _moveY = Input.GetAxis("Vertical");

        SetupDirectionByRotation();
        
        // Gravity
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y - (gravity * Time.deltaTime), 0);
        
        if (_move != 0)
        {
            _anim.SetBool(IsRunning, true);
        }
        else
        {
            _anim.SetBool(IsRunning, false);
        }
        
        if (_moveY != 0)
        {
            _anim.SetBool(IsIdle, false);
        }
        else
        {
            _anim.SetBool(IsIdle, true);
        }

        
        _anim.SetBool(IsAttacking, _isAttacking);
        _anim.SetBool(IsJumping, _isJumping);
        _anim.SetBool(IsClimbing, onWall);

        // Horizontal movement
        _rb.linearVelocity = new Vector3(_move * speed, _rb.linearVelocity.y, 0);
        
        // Jumping
        if (Input.GetButtonDown("Jump") && (_isGrounded || onWall))
        {
            AudioManager.instance.PlaySound(jumpSound, transform, 1f);
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, 0);
            _isGrounded = false;
            onWall = false;
        }

        if (_isGrounded || onWall)
        {
            _isJumping = false;
        }
        else {_isJumping = true;}

        if (!onWall)
        {
            _sr.flipY = false;
        }
        
        // Climbing
        _isTouchingWall = IsTouchingWall();
        if (_isTouchingWall)
        {
            onWall = true;
            _wallJumpTimer = wallJumpTime;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _moveY * climbSpeed, 0);
            if (Input.GetKey(KeyCode.W)) // Climb up with W
            {
                _sr.flipY = false;
            }
            else if (Input.GetKey(KeyCode.S)) // Climb down with S
            {
                _sr.flipY = true;
            }
        }
        else
        {
            _wallJumpTimer -= Time.deltaTime;
            if(_wallJumpTimer <= 0)
                _wallJumpTimer = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && (_wallJumpTimer > 0))
            WallJump();
        
        // Handle wall flip direction
        if (Input.GetAxis("Horizontal") > 0) // Moving right
        {
            _facingDirection = 1f;
        }
        else if (Input.GetAxis("Horizontal") < 0) // Moving left
        {
            _facingDirection = -1f;
        }
        
        // Falling
        if (_rb.linearVelocity.y < _fallingThreshold)
            _isFalling = true;
        else 
            _isFalling = false;
        _anim.SetBool(IsFalling, _isFalling);
        

        bool IsTouchingWall()
        {
            float wallCheckDistance = 0.8f;
            Vector3 direction = new Vector3(_facingDirection, 0, 0);
            // Dray the ray in scene view
            Debug.DrawRay(transform.position, direction * wallCheckDistance, Color.red);
            return Physics.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        }

        void WallJump()
        {
            AudioManager.instance.PlaySound(jumpSound, transform, 1f);
            Vector3 jumpDirection = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
            _rb.linearVelocity = new Vector3(jumpDirection.x * _wallJumpForce, jumpHeight, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            onWall = true;
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
            onWall = false;
        }
    }

    private void SetupDirectionByRotation()
    {
        if (_move < 0f)
        {
            _facingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (_move > 0f)
        {
            _facingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (_move == 0f)
        {
            switch (_facingRight)
            {
                case true:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case false:
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
    }

    private void PlayFootsteps()
    {
        AudioManager.instance.PlaySound(footSteps[Random.Range(0, footSteps.Length)], transform, 1f);
    }
}
