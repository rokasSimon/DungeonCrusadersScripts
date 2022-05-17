using Constants;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    private const string Master = "MasterVolume";
    [SerializeField] private Slider _masterAudio;

    private const string Effect = "EffectVolume";
    [SerializeField] private Slider _effectAudio;

    private const string Ambience = "AmbienceVolume";
    [SerializeField] private Slider _ambienceAudio;

    private const string Music = "MusicVolume";
    [SerializeField] private Slider _musicAudio;

    [SerializeField] private AudioMixer _mixer;

    void Start()
    {
        var playerSettings = Repository.LoadPlayerData();

        EnsureAudioSettingsAreSet(playerSettings);

        _masterAudio.minValue = FloatToVolume(0f);
        _masterAudio.maxValue = FloatToVolume(1f);

        _effectAudio.minValue = FloatToVolume(0f);
        _effectAudio.maxValue = FloatToVolume(1f);

        _ambienceAudio.minValue = FloatToVolume(0f);
        _ambienceAudio.maxValue = FloatToVolume(1f);

        _musicAudio.minValue = FloatToVolume(0f);
        _musicAudio.maxValue = FloatToVolume(1f);

        _masterAudio.SetValueWithoutNotify(playerSettings.AudioSettings.MasterVolume);
        _effectAudio.SetValueWithoutNotify(playerSettings.AudioSettings.EffectVolume);
        _ambienceAudio.SetValueWithoutNotify(playerSettings.AudioSettings.AmbienceVolume);
        _musicAudio.SetValueWithoutNotify(playerSettings.AudioSettings.MusicVolume);

        SetVolume(Master, playerSettings.AudioSettings.MasterVolume);
        SetVolume(Effect, playerSettings.AudioSettings.EffectVolume);
        SetVolume(Ambience, playerSettings.AudioSettings.AmbienceVolume);
        SetVolume(Music, playerSettings.AudioSettings.MusicVolume);
    }

    public void UpdateMasterVolume(float newVolume)
    {
        SetVolume(Master, newVolume);
    }

    public void UpdateEffectVolume(float newVolume)
    {
        SetVolume(Effect, newVolume);
    }

    public void UpdateAmbienceVolume(float newVolume)
    {
        SetVolume(Ambience, newVolume);
    }

    public void UpdateMusicVolume(float newVolume)
    {
        SetVolume(Music, newVolume);
    }

    public void SavePlayerSettings()
    {
        var playerSettings = Repository.LoadPlayerData();

        EnsureAudioSettingsAreSet(playerSettings);

        _mixer.GetFloat(Master, out var masterVolume);
        _mixer.GetFloat(Effect, out var effectVolume);
        _mixer.GetFloat(Ambience, out var ambienceVolume);
        _mixer.GetFloat(Music, out var musicVolume);

        playerSettings.AudioSettings.MasterVolume = masterVolume;
        playerSettings.AudioSettings.EffectVolume = effectVolume;
        playerSettings.AudioSettings.AmbienceVolume = ambienceVolume;
        playerSettings.AudioSettings.MusicVolume = musicVolume;

        Repository.SavePlayerData(playerSettings);
    }

    float FloatToVolume(float volume) => volume <= 0.0001 ? -80 : Mathf.Log(volume) * 20;

    void SetVolume(string param, float volume)
    {
        _mixer.SetFloat(param, volume);
    }

    void EnsureAudioSettingsAreSet(Player playerSettings)
    {
        if (playerSettings.AudioSettings == null)
        {
            playerSettings.AudioSettings = new AudioSettings
            {
                MasterVolume = FloatToVolume(1f),
                EffectVolume = FloatToVolume(1f),
                MusicVolume = FloatToVolume(1f),
                AmbienceVolume = FloatToVolume(1f)
            };

            Repository.SavePlayerData(playerSettings);
        }
    }
}
