using UnityEngine;

public class FireCracker : MonoBehaviour
{
    [Header("Fire Cracker Stats")]
    public float explosionRadius = 2f;
    public float fuseTime = 2f;
    public float stunDuration = 2f;
    
    [Header("Destroyable Layer")]
    public LayerMask destroyableLayer; // Assign this in the Inspector
    
    [Header("Effects")]
    public GameObject explosionEffect; // Optional, for visual effect

    public AudioClip throwSound;
    public AudioClip explodeSound;
    public AudioClip destroySound;

    void Start()
    {
        AudioManager.instance.PlaySound(throwSound, transform, 1f);
        // Auto-destroy after fuseTime seconds
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        // Optional: Spawn explosion VFX
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Find objects within the explosion radius
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider obj in hitObjects)
        {
            // Destroy objects with the "Destroyable" tag
            if (obj.CompareTag("Destroyable"))
            {
                AudioManager.instance.PlaySound(destroySound, transform, 1f);
                Destroy(obj.gameObject);
            }
            
            // Stun enemies with the "Enemy" tag
            if (obj.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = obj.GetComponent<EnemyAI>(); // Replace with your enemy script
                if (enemyAI != null)
                {
                    enemyAI.Stun(stunDuration);
                }
            }
        }
        
        AudioManager.instance.PlaySound(explodeSound, transform, 1f);
        Destroy(gameObject); // Destroy the firecracker itself
    }

    void OnDrawGizmos()
    {
        // Draw the explosion radius in the editor for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
