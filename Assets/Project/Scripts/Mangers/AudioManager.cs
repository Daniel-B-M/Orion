using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip FootSteps;
    public AudioClip Grunt;
    public AudioClip Happy;
    public AudioClip Vomit;
    public AudioClip Pour;
    public AudioClip Door;

    public AudioSource sfxSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayFootSteps()
    {
        sfxSource.PlayOneShot(FootSteps);
    }

    public void PlayGrunt()
    {
        sfxSource.PlayOneShot(Grunt);
    }

    public void PlayHappy()
    {
        sfxSource.PlayOneShot(Happy);
    }

    public void PlayVomit()
    {
        sfxSource.PlayOneShot(Vomit);
    }

    public void PlayPour()
    {
        sfxSource.PlayOneShot(Pour);
    }

    public void PlayDoor()
    {
        sfxSource.PlayOneShot(Door);
    }
}