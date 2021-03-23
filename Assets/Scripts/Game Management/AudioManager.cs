using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Cached References
    [SerializeField] float defaultFadeTime = 1.5f;
    [SerializeField] AudioSource[] soundEffects = null;
    [SerializeField] AudioSource[] backgroundMusic = null;

    // State Variables
    AudioSource currentSongPlaying = null;
    float currentSongVolume = 1f;
    bool isFading = false;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(int soundToPlay)
    {
        if (soundToPlay >= soundEffects.Length) { return; }

        soundEffects[soundToPlay].Play();
    }

    public void PlayMusic(int musicToPlay)
    {
        if (musicToPlay >= backgroundMusic.Length) { return; }

        if (currentSongPlaying == null)
        {
            StartCoroutine(FadeIn(musicToPlay, defaultFadeTime));
        }
        else
        {
            StartCoroutine(FadeOutAndIn(musicToPlay, defaultFadeTime));
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(defaultFadeTime));
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        while (isFading)
        {
            yield return null;
        }

        isFading = true;

        currentSongVolume = currentSongPlaying.volume;

        while (currentSongPlaying.volume > 0f)
        {
            currentSongPlaying.volume -= currentSongVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        currentSongPlaying.volume = 0f;

        currentSongPlaying.Stop();
        currentSongPlaying.volume = currentSongVolume;

        isFading = false;
    }

    private IEnumerator FadeOutAndIn(int songToPlay, float fadeTime)
    {
        while (isFading)
        {
            yield return null;
        }

        isFading = true;

        currentSongVolume = currentSongPlaying.volume;

        while (currentSongPlaying.volume > 0f)
        {
            currentSongPlaying.volume -= currentSongVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        currentSongPlaying.volume = 0f;

        currentSongPlaying.Stop();
        currentSongPlaying.volume = currentSongVolume;

        isFading = false;

        StartCoroutine(FadeIn(songToPlay, fadeTime));
    }

    private IEnumerator FadeIn(int songToPlay, float fadeTime)
    {
        while (isFading)
        {
            yield return null;
        }

        isFading = true;

        currentSongPlaying = backgroundMusic[songToPlay];
        currentSongVolume = currentSongPlaying.volume;

        currentSongPlaying.volume = 0f;

        currentSongPlaying.Play();

        while (currentSongPlaying.volume < currentSongVolume)
        {
            currentSongPlaying.volume += currentSongVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        currentSongPlaying.volume = currentSongVolume;

        isFading = false;
    }

    private void StopAllMusic()
    {
        foreach (AudioSource music in backgroundMusic)
        {
            music.Stop();
            currentSongPlaying = null;
        }
    }
}