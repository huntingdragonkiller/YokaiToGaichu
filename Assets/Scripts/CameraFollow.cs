using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (player)
        {
            Vector3 playerPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
