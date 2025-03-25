using UnityEngine;

public class GrappleUnlock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grapple"))
        {
            Grapple playerGrapple = other.gameObject.GetComponent<Grapple>();

            if (playerGrapple != null)
            {
                playerGrapple.UnlockGrapple();
                Destroy(gameObject);
            }
        }
    }
}
