using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    private Slider _bgm_Slider;
    private Slider _effect_Slider;
    private Slider _sensitivity_Slider;

    void Awake()
    {
        Transform soundOption   = transform.GetChild(0).GetChild(2);
        Transform controlOption = transform.GetChild(0).GetChild(3);

        _bgm_Slider         = soundOption.GetChild(1).GetChild(2).GetComponent<Slider>();
        _effect_Slider      = soundOption.GetChild(2).GetChild(2).GetComponent<Slider>();
        _sensitivity_Slider = controlOption.GetChild(1).GetChild(1).GetComponent<Slider>();

        _bgm_Slider        .value = 0.5f;
        _effect_Slider     .value = 0.5f;
        _sensitivity_Slider.value = 0.5f;
    }

    public void ResetUI()
    {

    }
    
    public void ToggleSettingUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Save()
    {
        InGameUIManager.Instance.ToggleSettingUI();
    }
}
