using UnityEngine;

public class FireCrackerUnlock : MonoBehaviour
{
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
