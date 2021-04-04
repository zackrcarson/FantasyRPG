using System.Collections;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float fadeTime = 1f;
    [SerializeField] AudioSource SFX = null;
    [SerializeField] AudioSource continueSound = null;
    [SerializeField] AudioSource newGameSound = null;

    // Cached References
    AudioSource music = null;

    // State Variables
    float currentSongVolume = 0.2f;

    private void Start()
    {
        music = GetComponent<AudioSource>();
    }

    public IEnumerator FadeOut()
    {
        currentSongVolume = music.volume;

        while (music.volume > 0f)
        {
            music.volume -= currentSongVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        music.volume = 0f;
        music.Stop();
    }

    public void PlayBeepSound()
    {
        SFX.Play();
    }

    public void PlayContinueSound()
    {
        continueSound.Play();
    }

    public void PlayNewGameSound()
    {
        newGameSound.Play();
    }
}
