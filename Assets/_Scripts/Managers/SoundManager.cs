using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _uiAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _fxAudioSource;

    [Header("SFX")]
    [SerializeField] private AudioClip _uiButtonClip;
    [SerializeField] private AudioClip[] _shootingClips;


    [Header("Music")]
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _battleMusic;
    [SerializeField] private AudioClip _winMusic;

    private void Awake()
    {
        Instance = this;
        Combat.UnitAttacked += PlayAttack;
    }

    private void OnDisable()
    {
        Combat.UnitAttacked -= PlayAttack;
    }
    public void PlayButtonHover()
    {
        _uiAudioSource.PlayOneShot(_uiButtonClip);
    }

    public void PlayAttack()
    {
        var index = Random.Range(0, _shootingClips.Length);
        _uiAudioSource.PlayOneShot(_shootingClips[index]);
    }

    public void PlayMenuMusic()
    {
        if (!_musicAudioSource.isPlaying || _musicAudioSource.clip != _mainMenuMusic)
        {
            _musicAudioSource.clip = _mainMenuMusic;
            _musicAudioSource.volume = 0.6f;
            _musicAudioSource.Play();
        }
    }

    public void PlayBattleMusic()
    {
        if (!_musicAudioSource.isPlaying || _musicAudioSource.clip != _battleMusic)
        {
            _musicAudioSource.clip = _battleMusic;
            _musicAudioSource.volume = 0.15f;
            _musicAudioSource.Play();
        }
    }
}
