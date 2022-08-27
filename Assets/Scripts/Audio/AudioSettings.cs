using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    #region Fields

    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] private RectTransform _windowPanel;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private TextMeshProUGUI _masterValueText;
    [SerializeField] private TextMeshProUGUI _musicValueText;
    [SerializeField] private TextMeshProUGUI _soundValueText;
    [SerializeField] private Button _closeButton;

    private const string MASTER_VOL = "MasterVol";
    private const string MUSIC_VOL = "MusicVol";
    private const string SOUND_VOL = "SoundVol";

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVol);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVol);
        _soundVolumeSlider.onValueChanged.AddListener(val => SetSoundVol(val));
        _closeButton.onClick.AddListener(() => SetSettingsWindowActive(false));

        SetSettingsWindowActive(false);
    }

    private void Start()
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

    private void OnDestroy()
    {
        _masterVolumeSlider.onValueChanged.RemoveAllListeners();
        _musicVolumeSlider.onValueChanged.RemoveAllListeners();
        _soundVolumeSlider.onValueChanged.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void SetMasterVol(float val)
    {
        _masterValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(MASTER_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(MASTER_VOL, Mathf.Log10(val) * 20);
    }

    private void SetMusicVol(float val)
    {
        _musicValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(MUSIC_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(MUSIC_VOL, Mathf.Log10(val) * 20);
    }

    private void SetSoundVol(float val, bool playSound = true)
    {
        _soundValueText.text = $"{val * 10}%";
        PlayerPrefs.SetFloat(SOUND_VOL, val);

        val = Mathf.Max(0.0001f, val / 10f);
        _audioMixer.SetFloat(SOUND_VOL, Mathf.Log10(val) * 20);

        if (playSound && _windowPanel.gameObject.activeSelf)
        {
            SoundManager.Instance?.PlaySound(References.HEALTH_SMALL_SOUND, false);
        }
    }

    public void SetSettingsWindowActive(bool isActive) => _windowPanel.gameObject.SetActive(isActive);

    #endregion
}
