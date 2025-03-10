using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    #region VARIABLES
    private Slider _bgmSlider;
    private Slider _effectSlider;
    private Slider _sensitivitySlider;

    private Toggle _bgmToggle;
    private Toggle _effectToggle;
    private Toggle _sensitivityToggle;

    private TextMeshProUGUI _sensitivityText;

    private Button _saveButton;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
    }

    public override void On()
    {
        gameObject.SetActive(true);
    }

    public override void Off()
    {
        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        Transform soundOption   = transform.GetChild(0).GetChild(2);
        Transform controlOption = transform.GetChild(0).GetChild(3);

        // 슬라이더 세팅
        {
            _bgmSlider         = soundOption  .GetChild(1).GetChild(2).GetComponent<Slider>();
            _effectSlider      = soundOption  .GetChild(2).GetChild(2).GetComponent<Slider>();
            _sensitivitySlider = controlOption.GetChild(1).GetChild(1).GetComponent<Slider>();

            _bgmSlider        .onValueChanged.AddListener(OnMasterVolumSliderChanged);
            _effectSlider     .onValueChanged.AddListener(OnEffectVolumeSliderChanged);
            _sensitivitySlider.onValueChanged.AddListener(OnSensitivitySliderChanged);

        }
        // 토글 세팅
        {
            _bgmToggle    = soundOption.GetChild(1).GetChild(1).GetComponent<Toggle>();
            _effectToggle = soundOption.GetChild(2).GetChild(1).GetComponent<Toggle>();

            _bgmToggle   .onValueChanged.AddListener(delegate { AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIToggle); });
            _effectToggle.onValueChanged.AddListener(delegate { AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIToggle); });
        }
        // 감도 텍스트 세팅
        {
            _sensitivityText = controlOption.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        // 저장 버튼 세팅
        {
            _saveButton = transform.GetChild(0).GetChild(4).GetComponent<Button>();

            _saveButton.onClick.AddListener(Save);
            _saveButton.onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));
        }
    }

    private void OnMasterVolumSliderChanged(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
    }

    private void OnEffectVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }

    private void OnSensitivitySliderChanged(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0.1f, 10f);

        _sensitivityText.text = clampedValue.ToString("F1");
    }

    public void Save()
    {
        AudioManager.Instance.bgmVolume = _bgmToggle   .isOn ? _bgmSlider   .value : 0;
        AudioManager.Instance.sfxVolume = _effectToggle.isOn ? _effectSlider.value : 0;

        InGameUIManager.Instance.ToggleSettingUI();
    }
    #endregion
}
