using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{ 
    [SerializeField] Sprite buttonImage; // 기본 저튼 이미지
    [SerializeField] Sprite pressedButtonImg; // 버튼을 눌렀을 때 바뀌는 이미지

    public Button soundButton; // 사운드 설정 버튼
    public Button keyButton; // 키 설정 버튼
    public GameObject soundSettings; // 사운드 설정 페이지
    public GameObject keySettings; // 키 설정 페이지


    [SerializeField] float masterVolume;
    [SerializeField] float effectVolume;

    public Toggle masterVolumeToggle; // 마스터 볼륨 체크박스
    public Slider masterVolumeSlider; // 마스터 볼륨 슬라이더
    public Toggle effectVolumeToggle; // 이펙트 볼륨 체크박스
    public Slider effectVolumeSlider; // 이펙트 볼륨 슬라이더


    private void Awake()
    {
        // 버튼 연결
        soundButton.onClick.AddListener(OnSoundButtonClick);
        keyButton.onClick.AddListener(OnKeySettingButtonClick);

        // 사운드 사용 유무 체크 토글 연결
        masterVolumeToggle.onValueChanged.AddListener(OnMasterVolumeToggleChanged);
        effectVolumeToggle.onValueChanged.AddListener(OnEffectVolumeToggleChanged);

        // 슬라이더 연결
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChanged);
        effectVolumeSlider.onValueChanged.AddListener(OnEffectVolumeSliderChanged);

        masterVolume = AudioManager.Instance.bgmVolume;
        effectVolume = AudioManager.Instance.sfxVolume;
    }

    public void OnSoundButtonClick()
    {
        soundButton.GetComponent<Image>().sprite= pressedButtonImg; // 눌린 버튼 이미지로 변경
        keyButton.GetComponent<Image>().sprite = buttonImage; // 기본 버튼 이미지로 변경

        soundSettings.SetActive(true); // 사운드 설정 페이지 활성화
        keySettings.SetActive(false); // 키 설정 페이지 비활성화
    }

    public void OnKeySettingButtonClick()
    {
        keyButton.GetComponent<Image>().sprite = pressedButtonImg; // 눌린 버튼 이미지로 변경
        soundButton.GetComponent<Image>().sprite = buttonImage; // 기본 버튼 이미지로 변경

        keySettings.SetActive(true);
        soundSettings.SetActive(false); // 사운드 설정 페이지 비활성화
    }

    private void OnMasterVolumeToggleChanged(bool isOn)
    {
        masterVolumeSlider.interactable = isOn; // 슬라이더 활성화/비활성화
        if (!isOn)
        {
            masterVolumeSlider.value = 0; // 체크 해제 시 0으로 설정
            AudioManager.Instance.SetBgmVolume(0); // BGM 볼륨 0으로 설정
        }
        else
        {
            masterVolumeSlider.value = masterVolume; // 체크 시 현재 볼륨으로 설정
        }
    }

    private void OnEffectVolumeToggleChanged(bool isOn)
    {
        effectVolumeSlider.interactable = isOn; // 슬라이더 활성화/비활성화
        if (!isOn)
        {
            effectVolumeSlider.value = 0; // 체크 해제 시 0으로 설정
            AudioManager.Instance.SetSfxVolume(0); // SFX 볼륨 0으로 설정
        }
        else
        {
            effectVolumeSlider.value = effectVolume; // 체크 시 현재 볼륨으로 설정
        }
    }

    private void OnMasterVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetBgmVolume(value); // 슬라이더 값으로 변화
    }

    private void OnEffectVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value); // 슬라이더 값으로 변화
    }

    public void OnSaveButtonClick()
    {
        // 설정 저장 처리
        AudioManager.Instance.bgmVolume = masterVolumeToggle.isOn ? masterVolumeSlider.value : 0;
        AudioManager.Instance.sfxVolume = effectVolumeToggle.isOn ? effectVolumeSlider.value : 0;

        Debug.Log("Settings Saved");
    }

    public void OnCloseButtonClick()
    {
        // CommonButtonManager.cs에 없는 내용들
        // 소리 & 키 정상화
        AudioManager.Instance.SetBgmVolume(AudioManager.Instance.bgmVolume);
        AudioManager.Instance.SetSfxVolume(AudioManager.Instance.sfxVolume);
    }
}
