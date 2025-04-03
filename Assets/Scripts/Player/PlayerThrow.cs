using UnityEngine;
using UnityEngine.UI;

public class PlayerThrow : MonoBehaviour
{
    public GameObject firecrackerPrefab;
    public Transform throwPoint;
    public float throwForce = 5f;
    public float coolDownTime = 4f;
    public Image firecrackerImage;
    public Sprite fireCrackerSprite;
    
    private float _nextThrowTime;
    [HideInInspector]
    public bool firecrackerUnlocked;

    void Start()
    {
        // Load saved unlock state (1 = unlocked, 0 = locked)
        firecrackerUnlocked = PlayerPrefs.GetInt("firecrackerUnlocked", 0) == 1;
    }

    void Update()
    {
        if (firecrackerUnlocked) 
        {
            firecrackerImage.sprite = fireCrackerSprite;
            if (Input.GetKeyDown(KeyCode.F) && Time.time >= _nextThrowTime) // Change keybind as needed
            {
                ThrowFirecracker();
                _nextThrowTime = Time.time + coolDownTime;
            }
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
        firecrackerUnlocked = true;
        PlayerPrefs.SetInt("FirecrackerUnlocked", 1); // Save unlock state
        PlayerPrefs.Save();
    }
}