using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider MasterSlider;
    public Slider BgmSlider;
    public Slider SfxSlider;
    public Button SaveButton;

    [Header("UI Texts")]
    public TMP_Text MasterValueText;
    public TMP_Text BgmValueText;
    public TMP_Text SfxValueText;

    [Header("Audio Sources")]
    public AudioSource BgmSource;
    public AudioSource SfxSource;

    void Start()
    {
        // 이벤트 등록
        MasterSlider.onValueChanged.AddListener(UpdateMasterValue);
        BgmSlider.onValueChanged.AddListener(UpdateBgmValue);
        SfxSlider.onValueChanged.AddListener(UpdateSfxValue);

        // 초기값 표시
        UpdateMasterValue(MasterSlider.value);
        UpdateBgmValue(BgmSlider.value);
        UpdateSfxValue(SfxSlider.value);
    }

    private void UpdateMasterValue(float value)
    {
        MasterValueText.text = Mathf.RoundToInt(value * 100).ToString();
        // 전체 볼륨에 반영하려면:
        AudioListener.volume = value;
    }

    private void UpdateBgmValue(float value)
    {
        BgmValueText.text = Mathf.RoundToInt(value * 100).ToString();
        if (BgmSource != null)
            BgmSource.volume = value;
    }

    private void UpdateSfxValue(float value)
    {
        SfxValueText.text = Mathf.RoundToInt(value * 100).ToString();
        if (SfxSource != null)
            SfxSource.volume = value;
    }
}
