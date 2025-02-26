using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public GameObject firecrackerPrefab;
    public Transform throwPoint;
    public float throwForce = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Change keybind as needed
        {
            ThrowFirecracker();
        }
    }

    void ThrowFirecracker()
    {
        GameObject firecracker = Instantiate(firecrackerPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = firecracker.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.linearVelocity = (transform.right + transform.up) * throwForce; // Adjust for player direction
        }
            
    }
}