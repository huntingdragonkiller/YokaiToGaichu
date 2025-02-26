using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStats _player;

    public GameObject slashEffect;
    public Collider attackHitbox;
    public float attackDuration = 0.2f;

    private bool isAttacking = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackHitbox.enabled = true;
        slashEffect.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        slashEffect.SetActive(true);

        yield return new WaitForSeconds(attackDuration);
        
        attackHitbox.enabled = false;
        slashEffect.SetActive(false);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy && _player)
            {
                Debug.Log($"Dealing {_player.currentDamage} damage to {other.gameObject.name}");
                enemy.TakeDamage(_player.currentDamage);
            }
            else
            {
                Debug.LogError("EnemyStats or PlayerStats is missing!");
            }
        }
    }
}
