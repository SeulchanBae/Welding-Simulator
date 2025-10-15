using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    public Toggle hapticToggle;

    void Start()
    {
        hapticToggle.isOn = PlayerPrefs.GetInt("HapticEnabled", 1) == 1;
        hapticToggle.onValueChanged.AddListener(SetHaptic);
    }

    public void SetHaptic(bool enabled) 
    {
        PlayerPrefs.SetInt("HapticEnabled", enabled ? 1 : 0);
        Debug.Log($"Áøµ¿: {enabled}");
    }
}