
using UnityEngine;

public class JPQuickSound
{
    private static AudioSource audioSource;

    private static void SetupSource()
    {
        var obj = new GameObject();
        audioSource = obj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.volume = 1;
    }
    
    public static void PlayQuickSound(AudioClip clip, float volume = 1)
    {
        if(!audioSource)
            SetupSource();
        
        audioSource.PlayOneShot(clip, volume);
    }
}
