using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneSoftGame.Tools;
using DG.Tweening;


public class SoundManager : PersistentSingleton<SoundManager> {

    public enum AudioSound
    {
        Audio = 0,
        Sound = 1
    }

    [SerializeField]
    private AudioSource _bgmSource;

    [SerializeField]
    private AudioSource _sfxSource;

    public AudioData audioData;
    private Audio _audio = null;

    private bool _hasExtensionClips = false;

    public static int AudioSettings = 0;
    public static int SoundSettings = 0;

    private void Start()
    {
        ChangeResourceSound();
        

        AudioSettings = GetAudioSoundSettings(AudioSound.Audio);
        SoundSettings = GetAudioSoundSettings();

        PlayBGM();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public int GetAudioSoundSettings(AudioSound t = AudioSound.Sound)
    {
        int val = 0;
        switch (t)
        {
            case AudioSound.Audio:
                val = PlayerPrefs.GetInt(GameKey.MUSIC, 1);
                break;
            case AudioSound.Sound:
                val = PlayerPrefs.GetInt(GameKey.SOUND, 1);
                break;
            default:
                break;
        }

        return val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsBGMPlaying()
    {
        return _bgmSource.isPlaying;
    }

    /// <summary>
    /// Play normal, it will check settings
    /// </summary>
    public void PlayBGM()
    {
        if (AudioSettings == 0) return;

        if (_bgmSource.volume == 0f)
        {
            _bgmSource.volume = 0.5f;
        }

        if (_bgmSource.isPlaying) return;

        if (_audio != null)
        {
            if (_bgmSource.clip != _audio.bgmClip)
                _bgmSource.clip = _audio.bgmClip;
        }

        _bgmSource.Play();
    }

    /// <summary>
    /// Force to play BGM
    /// </summary>
    public void ForcePlayBGM()
    {
        PlayerPrefs.SetInt(GameKey.MUSIC, 1);
        AudioSettings = 1;

        PlayBGM();
    }

    /// <summary>
    /// Stop BGM
    /// </summary>
    public void StopBGM()
    {
        PlayerPrefs.SetInt(GameKey.MUSIC, 0);
        AudioSettings = 0;

        _bgmSource.Stop();
    }

    /// <summary>
    /// Stop sound effect
    /// </summary>
    public void StopSoundFx()
    {
        PlayerPrefs.SetInt(GameKey.SOUND, 0);
        SoundSettings = 0;
    }

    /// <summary>
    /// Return true if sound is turn on.
    /// </summary>
    public bool OnOffSoundFx()
    {
        var on = SoundSettings == 1;

        if(on)
        {
            StopSoundFx();
        } else
        {
            PlayerPrefs.SetInt(GameKey.SOUND, 1);
            SoundSettings = 1;
        }

        return !on;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public void PlayClip(AudioType type)
    {

        if (SoundSettings == 0) return;

        if (_audio != null)
        {
            switch (type)
            {
                case AudioType.Click:
                    _sfxSource.PlayOneShot(audioData.commonAudio.clickClip);
                    break;
                case AudioType.Spawn:
                    _sfxSource.PlayOneShot(audioData.commonAudio.spawnBoardClip);
                    break;
                case AudioType.Chosen:
                    _sfxSource.PlayOneShot(_audio.chosenBoardClip);
                    break;
                case AudioType.Place:
                    _sfxSource.PlayOneShot(_audio.placeBoardClip);
                    break;
                case AudioType.Combo:
                    _sfxSource.PlayOneShot(_audio.comboClip);
                    break;
                case AudioType.Lose:
                    _bgmSource.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        _bgmSource.Stop();
                    });

                    _sfxSource.PlayOneShot(audioData.commonAudio.loseClip);
                    break;
                case AudioType.ComboOne:
                    //_sfxSource.PlayOneShot(_audio.comboAudio.comboOne);
                    break;
                case AudioType.ComboTwo:
                    //_sfxSource.PlayOneShot(_audio.comboAudio.comboTwo);
                    break;
                case AudioType.ComboThree:
                    //_sfxSource.PlayOneShot(_audio.comboAudio.comboThree);
                    break;
                case AudioType.ComboFour:
                    //_sfxSource.PlayOneShot(_audio.comboAudio.comboFour);
                    break;
                default:
                    break;
            }

        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="order"></param>
    public void PlayExtensionClip(int order)
    {
        if (SoundSettings == 0) return;

        if (!_hasExtensionClips || _audio == null) return;
        if (order < 0 || order >= 3) return;

        if(order == 1)
        {
            if (_audio.extendAudio.clipOne)
            {
                _sfxSource.PlayOneShot(_audio.extendAudio.clipOne);
            }
        } else if(order == 2)
        {
            if (_audio.extendAudio.clipTwo)
            {
                _sfxSource.PlayOneShot(_audio.extendAudio.clipTwo);
            }
        } else
        {
            if (_audio.extendAudio.clipThree)
            {
                _sfxSource.PlayOneShot(_audio.extendAudio.clipThree);
            }
        }
    }

    /// <summary>
    /// We need to change resource of the soundManager each time we change theme
    /// </summary>
    /// <param name="needReBgm">is need to replay BGM</param>
    public void ChangeResourceSound(bool needReBgm = false)
    {
        _audio = audioData.data[1];

        _hasExtensionClips = _audio.hasExtensionClips;

        if (needReBgm)
        {
            _bgmSource.Stop();
            ForcePlayBGM();
        }
    }
}
