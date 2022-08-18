using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _sounds;
    public static SoundManager Instance;

    [SerializeField] private ObjectPool _soundPool;
    private readonly Dictionary<string, AudioClip> _nameToSound = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
                Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_soundPool == null)
            _soundPool = GetComponent<ObjectPool>();
    }

    private void Start()
    {
        foreach (var sound in _sounds)
        {
            _nameToSound.Add(sound.name, sound);
        }
    }

    public void AddSounds(List<AudioClip> soundsToAdd)
    {
        foreach (var sound in soundsToAdd)
        {
            _nameToSound.Add(sound.name, sound);
        }
    }

    public void RemoveSounds(List<AudioClip> soundsToAdd)
    {
        foreach (var sound in soundsToAdd)
        {
            _nameToSound.Remove(sound.name);
        }
    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (clip != null)
        {
            _soundPool.GetObject().GetComponent<SoundFx>().Play(clip, loop);
        }
    }

    public void PlaySound(string soundName, bool loop = false)
    {
        var clip = _nameToSound[soundName];
        if (clip != null)
        {
            PlaySound(clip, loop);
        }
    }

    public void StopSound(string soundName)
    {
        foreach (var sound in _soundPool.GetComponentsInChildren<SoundFx>())
        {
            if (sound.GetComponent<AudioSource>().clip == _nameToSound[soundName])
            {
                sound.GetComponent<PooledObject>().Pool.ReturnObject(sound.gameObject);
            }
        }
    }
}
