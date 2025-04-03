using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    public Transform leftPatrolPoint, rightPatrolPoint;
    public LayerMask groundLayer;

    [Header("Jump Settings")]
    public float jumpForce = 3f; // Vertical force for jump
    public float forwardForce = 5f; // Forward force for jump
    public float jumpCooldown = 2f; // Time between jumps
    private bool _isJumping;
    
    [Header("Vision Settings")]
    public float visionRange = 5f;
    public float visionAngle = 45f; // Angle of vision cone
    public Transform visionOrigin;  // Position where the enemy "sees"
    
    private Rigidbody _rb;
    private bool _movingRight = true;
    private Transform _player;
    private PlayerStats _playerStats;
    private EnemyStats _enemy;
    private bool _playerDetected;
    
    private bool _isStunned;
    private float _stunEndTime;
    
    public float patrolPauseDuration = 2f;
    private bool _isPaused;

    public AudioClip alertSound;
    private bool _soundPlayed;
    
    private Animator _animator;
    private bool _isMoving;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _enemy = GetComponent<EnemyStats>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Find player by tag
        _playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
        _animator = GetComponent<Animator>();

        StartCoroutine(JumpRoutine());
    }
    
    void Update()
    {
        if (_isStunned && Time.deltaTime >= _stunEndTime)
        {
            _isStunned = false;
        }
        else if (!_isStunned)
        {
            DetectPlayer();

            if (_playerDetected)
            {
                _playerStats.currentDamage = _playerStats.defaultDamage;
                ChasePlayer();
            }
            else
            {
                jumpCooldown = 2f;
                _playerStats.currentDamage = _playerStats.defaultDamage * 100;
            }
        }

        if (_rb.linearVelocity.x is > 0 or < 0)
            _isMoving = true;
        else if (_rb.angularVelocity.x == 0)
            _isMoving = false;
        
        _animator.SetBool(IsMoving, _isMoving);
    }

    
    void DetectPlayer()
    {
        if (!_player) return;
        
        // Get the forward direction of the enemy
        Vector3 forwardDirection = default;
        if (_movingRight)
            forwardDirection = transform.right;
        else if (!_movingRight)
            forwardDirection *= -1;

        Vector3 directionToPlayer = (_player.position - visionOrigin.position).normalized;
        float distanceToPlayer = Vector3.Distance(visionOrigin.position, _player.position);

        // Check if player is within vision range
        if (distanceToPlayer <= visionRange)
        {
            // Calculate the angle between the enemy’s forward direction and the player's direction
            float angleToPlayer = Vector3.Angle(forwardDirection, directionToPlayer);
            // Check if the player is within the vision cone
            if (angleToPlayer <= visionAngle / 2f)
            {
                // Perform a Raycast to check if there’s a clear line of sight
                RaycastHit hit;
                if (Physics.Raycast(visionOrigin.position, directionToPlayer, out hit, visionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        PlaySoundOnce();
                        _playerDetected = true;
                        return;
                    }
                }
            }
        }
        _playerDetected = false;
        _soundPlayed = false;
    }

    void ChasePlayer()
    {
        if (!_player) return;
        
        _playerStats.currentDamage = _playerStats.defaultDamage;

        jumpCooldown = 1f;
        float direction = (_player.position.x > transform.position.x) ? 1f : -1f;

        // Flip sprite to face player
        if (direction > 0 && !_movingRight) Flip();
        if (direction < 0 && _movingRight) Flip();
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            if (!_isJumping && !_isPaused)
                Jump();
            yield return new WaitForSeconds(jumpCooldown);
        }
    }

    void Jump()
    {
        if (_isJumping) return;
        
        _isJumping = true;
        
        float moveDirection = _movingRight ? 1f : -1f;
        
        if(_playerDetected)
            moveDirection = (_player.position.x > transform.position.x) ? 1f : -1f;
        
        _rb.linearVelocity = new Vector3(forwardForce * moveDirection, jumpForce, 0);
        Invoke(nameof(Land), jumpCooldown / 2);
    }

    void Land()
    {
        _isJumping = false;

        if (_movingRight && transform.position.x >= rightPatrolPoint.position.x)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }
        else if (!_movingRight && transform.position.x <= leftPatrolPoint.position.x)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }

        //RaycastHit hit;
        /*
        if(Physics.Raycast(transform.position, Vector3.right * (_movingRight ? 1 : -1), out hit, groundLayer))
            Flip();
            */
    }
    
    void Flip()
    {
        _movingRight = !_movingRight;
        if (_rb.linearVelocity.x > 0)
        {
            //_movingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (!_movingRight)
        {
            //_movingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        //transform.Rotate(0, 180, 0);

        if (_rb.linearVelocity.x == 0)
        {
            switch (_movingRight)
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
    
    public void Stun(float duration)
    {
        _isStunned = true;
        _stunEndTime = Time.deltaTime + duration;
        StopEnemyMovement(); // Stops movement when stunned
    }
    
    void StopEnemyMovement()
    {
        // If using Rigidbody2D for movement, stop velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    
    IEnumerator PauseAtPatrolPoint()
    {
        _isPaused = true;
        _rb.linearVelocity = Vector3.zero;  // Stop moving while paused
        yield return new WaitForSeconds(patrolPauseDuration);
        Flip();  // Flip direction after pause
        _isPaused = false;
    }

    void PlaySoundOnce()
    {
        if (!_soundPlayed)
        {
            AudioManager.instance.PlaySound(alertSound, transform, 1f);
            _soundPlayed = true;
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw patrol range
        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftPatrolPoint.position, rightPatrolPoint.position);
        
        if (visionOrigin)
        {
            // Draw vertical vision cone
            Gizmos.color = Color.red;
            Vector3 forward = transform.right;
            Vector3 leftBoundary = default;
            Vector3 rightBoundary = default;
            if (_movingRight)
            {
                leftBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * (forward);
                rightBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * (forward);
            }
            else if (!_movingRight)
            {
                leftBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * (forward);
                rightBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * (forward);
            }
            
            Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + leftBoundary * visionRange);
            Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + rightBoundary * visionRange);
        }
    }
}
