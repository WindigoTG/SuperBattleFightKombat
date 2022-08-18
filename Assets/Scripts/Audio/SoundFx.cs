using UnityEngine;

public class SoundFx : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, bool loop = false)
    {
        if (clip == null)
        {
            return;
        }
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
        Invoke("DisableSoundFx", clip.length + 0.1f);
    }

    private void DisableSoundFx()
    {
        GetComponent<PooledObject>().Pool.ReturnObject(gameObject);
    }
}
