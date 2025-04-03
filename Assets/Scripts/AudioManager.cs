using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource sound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    public void PlaySound(AudioClip clip, Transform spawnTransform, float volume)
    {
        // spawn in game object
        AudioSource audioSource = Instantiate(sound, spawnTransform.position, Quaternion.identity);
        
        // assign the audioClip
        audioSource.clip = clip;
        
        // assign volume
        audioSource.volume = volume;
        
        // pitch adjustment
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        
        // play sound
        audioSource.Play();
        
        // get length of sound FX clip
        float clipLength = audioSource.clip.length;
        
        // Destroy the clip after it is done
        Destroy(audioSource.gameObject, clipLength);
    }
}
