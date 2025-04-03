using UnityEngine;

public class BGMSingle : MonoBehaviour
{
    private static BGMSingle instance;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
