using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AudioManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxPreviewSource;

    [Header("Preview Audio Clips")]
    public AudioClip weldingSound;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);

        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        UpdateMasterText(masterVolume);
        UpdateBGMText(bgmVolume);
        UpdateSFXText(sfxVolume);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // SFX 슬라이더에만 드래그 이벤트 추가
        AddSliderDragEvents(sfxSlider);

        ApplyVolumes();

        // Preview Source가 시작 시 재생 안 되도록
        if (sfxPreviewSource != null)
        {
            sfxPreviewSource.Stop();
        }
    }

    void AddSliderDragEvents(Slider slider)
    {
        EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = slider.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryBegin = new EventTrigger.Entry();
        entryBegin.eventID = EventTriggerType.PointerDown;
        entryBegin.callback.AddListener((data) => { OnSliderDragStart(); });
        trigger.triggers.Add(entryBegin);

        EventTrigger.Entry entryEnd = new EventTrigger.Entry();
        entryEnd.eventID = EventTriggerType.PointerUp;
        entryEnd.callback.AddListener((data) => { OnSliderDragEnd(); });
        trigger.triggers.Add(entryEnd);
    }

    void OnSliderDragStart()
    {
        if (sfxPreviewSource != null && weldingSound != null)
        {
            sfxPreviewSource.clip = weldingSound;
            sfxPreviewSource.loop = true;
            sfxPreviewSource.volume = masterVolume * sfxVolume;
            sfxPreviewSource.Play();
        }
    }

    void OnSliderDragEnd()
    {
        if (sfxPreviewSource != null)
        {
            sfxPreviewSource.Stop();
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        UpdateMasterText(volume);
        ApplyVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
        UpdateBGMText(volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        UpdateSFXText(volume);
        ApplyVolumes();
    }

    void ApplyVolumes()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = masterVolume * bgmVolume;
        }

        if (sfxPreviewSource != null && sfxPreviewSource.isPlaying)
        {
            sfxPreviewSource.volume = masterVolume * sfxVolume;
        }
    }

    void UpdateMasterText(float volume)
    {
        masterText.text = $"{(int)(volume * 100)}";
    }

    void UpdateBGMText(float volume)
    {
        bgmText.text = $"{(int)(volume * 100)}";
    }

    void UpdateSFXText(float volume)
    {
        sfxText.text = $"{(int)(volume * 100)}";
    }
}