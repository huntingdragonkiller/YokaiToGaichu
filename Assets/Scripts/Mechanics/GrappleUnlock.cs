using Unity.VisualScripting;
using UnityEngine;

public class GrappleUnlock : MonoBehaviour
{
    private Grapple _grapple;
    void Start()
    {
        _grapple = GameObject.FindGameObjectWithTag("Grapple").GetComponent<Grapple>();
        if (_grapple.grappleUnlocked == true)
        {
            Debug.Log("Grapple unlocked");
            Destroy(gameObject);
        }
    }
    
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
