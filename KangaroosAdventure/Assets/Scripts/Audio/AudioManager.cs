using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Sound
{
    BUSH, FLAG, EXPLOSION, HAPPY_CHICKEN, EATING, STARVING
}



public class AudioManager : MonoBehaviour
{
    private const float GAME_MUSIC_VOLUME_GAME_END = 0.02f;
    private const float GAME_MUSIC_VOLUME_INGAME = 0.03f;

    public bool soundsActive = false;
    public bool musicActive = false;

    public Button soundsActiveButton;
    public Button musicActiveButton;

    public Sprite activeSoundSprite;
    public Sprite inactiveSoundSprite;


    public AudioSource backgroundMusic;
    public AudioSource menuMusic;

    public AudioSource explosionSound;
    public AudioSource happyChickenSound;
    public AudioSource seedSound;
    public AudioSource flagSound;
    public AudioSource screamingSound;
    public List<AudioSource> bushSounds;


    private static AudioManager instance;

    public static AudioManager GetInstance()
    {
        return instance;
    }
    
    public void Init()
    {
        instance = this;
        print(PlayerPrefs.GetInt("SoundsOn"));
        if (PlayerPrefs.GetInt("SoundsOn") == 1)
            ToggleSounds();

        if (PlayerPrefs.GetInt("MusicOn") == 1)
            ToggleMusic();
    }

    //public void InitToggleSprites()
    //{
        
    //}

    public void PlayGameMusic()
    {
        backgroundMusic.volume = GAME_MUSIC_VOLUME_INGAME;

        happyChickenSound.Stop();
        menuMusic.Stop();
        if (musicActive && !backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    public void PlayWinSounds()
    {
        backgroundMusic.volume = GAME_MUSIC_VOLUME_GAME_END;

        PlaySound(Sound.HAPPY_CHICKEN);
    }

    public void PlayLossSounds()
    {
        backgroundMusic.volume = GAME_MUSIC_VOLUME_GAME_END;
    }

    public void PlayMenuMusic()
    {
        happyChickenSound.Stop();
        backgroundMusic.Stop();
        if (musicActive && !menuMusic.isPlaying)
            menuMusic.Play();
    }

    public void PlaySound(Sound sound)
    {
        if (!soundsActive)
            return;

        AudioSource toPlay;

        switch (sound)
        {
            case Sound.BUSH:
                toPlay = bushSounds[Random.Range(0, 4)];
                break;

            case Sound.FLAG:
                toPlay = flagSound;
                break;

            case Sound.EATING:
                toPlay = seedSound;
                break;

            case Sound.EXPLOSION:
                toPlay = explosionSound;
                break;

            case Sound.HAPPY_CHICKEN:
                toPlay = happyChickenSound;
                break;

            case Sound.STARVING:
                toPlay = screamingSound;
                break;

            default:
                throw new Exception("Sound not found!");
        }

        toPlay.Play();
    }
    

    public void ToggleSounds()
    {
        soundsActive = !soundsActive;
        PlayerPrefs.SetInt("SoundsOn", soundsActive ? 1 : 0);
        soundsActiveButton.GetComponent<Image>().sprite = soundsActive ? activeSoundSprite : inactiveSoundSprite;
    }

    public void ToggleMusic()
    {
        musicActive = !musicActive;
        PlayerPrefs.SetInt("MusicOn", musicActive ? 1 : 0);

        if (musicActive)
        {
            musicActiveButton.GetComponent<Image>().sprite = activeSoundSprite;
            menuMusic.Play();
        }
        else
        {
            musicActiveButton.GetComponent<Image>().sprite = inactiveSoundSprite;
            menuMusic.Stop();
        }
    }
}
