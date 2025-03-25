using Unity.VisualScripting;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public GameObject firecrackerPrefab;
    public Transform throwPoint;
    public float throwForce = 5f;
    public float coolDownTime = 4f;
    
    private float _nextThrowTime;
    private bool _firecrackerUnlocked;

    void Start()
    {
        // Load saved unlock state (1 = unlocked, 0 = locked)
        _firecrackerUnlocked = PlayerPrefs.GetInt("firecrackerUnlocked", 0) == 1;
    }

    void Update()
    {
        if (_firecrackerUnlocked && Input.GetKeyDown(KeyCode.F) && Time.time >= _nextThrowTime) // Change keybind as needed
        {
            ThrowFirecracker();
            _nextThrowTime = Time.time + coolDownTime;
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
    
    public void UnlockFirecracker()
    {
        _firecrackerUnlocked = true;
        PlayerPrefs.SetInt("FirecrackerUnlocked", 1); // Save unlock state
        PlayerPrefs.Save();
    }
}