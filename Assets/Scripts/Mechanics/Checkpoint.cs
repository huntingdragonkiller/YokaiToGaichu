using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCheckpoint playerCheckpoint = other.GetComponent<PlayerCheckpoint>();
            if (playerCheckpoint != null)
            {
                playerCheckpoint.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint set!");
            }
        }
    }
}
