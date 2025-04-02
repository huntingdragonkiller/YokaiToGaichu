using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    private Vector3 _respawnPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _respawnPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Respawn();
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        _respawnPoint = newCheckpoint;
    }

    public void Respawn()
    {
        transform.position = _respawnPoint;
        Debug.Log("Respawn");
    }
}
