using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform leftPatrolPoint, rightPatrolPoint;
    public LayerMask groundLayer;

    [Header("Vision Settings")]
    public float visionRange = 5f;
    public float visionAngle = 45f; // Angle of vision cone
    public Transform visionOrigin;  // Position where the enemy "sees"
    
    private float _moveSpeed;
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
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _enemy = GetComponent<EnemyStats>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;  // Find player by tag
        _playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
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
                _playerStats.currentDamage = _playerStats.defaultDamage * 100;
                Patrol();
            }
        }
    }

    void Patrol()
    {
        if (_isPaused)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
            return;
        }
        
        _moveSpeed = _enemy.currentMoveSpeed;
        float moveDirection = _movingRight ? 1f : -1f;
        _rb.linearVelocity = new Vector3(_moveSpeed * moveDirection, _rb.linearVelocity.y, 0);

        // Check if the enemy reached a patrol point
        if (_movingRight && transform.position.x >= rightPatrolPoint.position.x)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }
        else if (!_movingRight && transform.position.x <= leftPatrolPoint.position.x)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }

        // Check for walls using a raycast
        Vector3 direction = _movingRight ? Vector3.right : Vector3.left;
        if (Physics.Raycast(transform.position, direction, 0.5f, groundLayer))
        {
            Flip();
        }
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

        _moveSpeed = _enemy.currentMoveSpeed * 1.5f;
        float direction = (_player.position.x > transform.position.x) ? 1f : -1f;
        _rb.linearVelocity = new Vector3(_moveSpeed * direction, _rb.linearVelocity.y, 0);

        // Flip sprite to face player
        if (direction > 0 && !_movingRight) Flip();
        if (direction < 0 && _movingRight) Flip();
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
