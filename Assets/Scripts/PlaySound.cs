using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySound : MonoBehaviour
{
    AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE()
    {
        audioSource.PlayOneShot(audioSource.clip);
        
    }
}
