using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Fields

    public static MusicManager Instance;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private List<AudioClip> _matchMusic;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    #endregion


    #region Properties

    public int MatchMusicCount => _matchMusic.Count;

    #endregion


    #region Methods

    public void PLayMenuMusic()
    {
        if (_audioSource.clip == _menuMusic && _audioSource.isPlaying)
            return;

        _audioSource.clip = _menuMusic;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void PlayRandomMatchMusic()
    {
        if (MatchMusicCount == 0)
            return;

        var idx = Random.Range(0, MatchMusicCount);

        if (_audioSource.clip == _matchMusic[idx] && _audioSource.isPlaying)
            return;

        _audioSource.clip = _matchMusic[idx];
        _audioSource.Play();
    }

    public void PlayMatchMusic(int musicClipIndex)
    {
        if (MatchMusicCount == 0)
            return;

        if (_audioSource.clip == _matchMusic[musicClipIndex] && _audioSource.isPlaying)
            return;

        _audioSource.clip = _matchMusic[musicClipIndex];
        _audioSource.Play();
    }

    #endregion
}
