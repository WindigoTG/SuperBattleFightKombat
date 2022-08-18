using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private TextMeshProUGUI _masterValueText;
    [SerializeField] private TextMeshProUGUI _musicValueText;
    [SerializeField] private TextMeshProUGUI _soundValueText;

    private const string MASTER_VOL = "MasterVol";
    private const string MUSIC_VOL = "MusicVol";
    private const string SOUND_VOL = "SoundVol";

    private void Awake()
    {
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVol);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVol);
        _soundVolumeSlider.onValueChanged.AddListener(val => SetSoundVol(val));
    }

    // Start is called before the first frame update
    void Start()
    {
        var masterVol = PlayerPrefs.GetFloat(MASTER_VOL, 10);
        _masterVolumeSlider.value = masterVol;
        SetMasterVol(masterVol);

        var musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, 10);
        _musicVolumeSlider.value = musicVol;
        SetMusicVol(musicVol);

        var soundVol = PlayerPrefs.GetFloat(SOUND_VOL, 10);
        _soundVolumeSlider.value = soundVol;
        SetSoundVol(soundVol, false);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void SetMasterVol(float val)
    {
        _masterValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(MASTER_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(MASTER_VOL, Mathf.Log10(val) * 20);
    }

    public void SetMusicVol(float val)
    {
        _musicValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(MUSIC_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(MUSIC_VOL, Mathf.Log10(val) * 20);
    }

    public void SetSoundVol(float val, bool playSound = true)
    {
        _soundValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(SOUND_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(SOUND_VOL, Mathf.Log10(val) * 20);

        if (playSound)
        {
            SoundManager.Instance?.StopSound("Hurt");
            SoundManager.Instance?.PlaySound("Hurt", false);
        }
    }
}
