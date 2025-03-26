using UnityEngine;

public class FireCrackerUnlock : MonoBehaviour
{
    private PlayerThrow _fireCracker;
    void Start()
    {
        _fireCracker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerThrow>();
        if (_fireCracker.firecrackerUnlocked)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerThrow playerFirecracker = other.gameObject.GetComponent<PlayerThrow>();

            if (playerFirecracker != null)
            {
                playerFirecracker.UnlockFirecracker();
                Destroy(gameObject);
            }
        }
    }
}
