using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] float masterVolume;
    [SerializeField] float effectVolume;

    public GameObject soundSettings; // 사운드 설정 페이지
    public GameObject keySettings; // 키 설정 페이지

    public Toggle masterVolumeToggle; // 마스터 볼륨 체크박스
    public Slider masterVolumeSlider; // 마스터 볼륨 슬라이더
    public Toggle effectVolumeToggle; // 이펙트 볼륨 체크박스
    public Slider effectVolumeSlider; // 이펙트 볼륨 슬라이더


    private void Start()
    {
        
    }

    public void OnSoundButtonClick()
    {
        soundSettings.SetActive(true);
        keySettings.SetActive(false); // 키 설정 페이지 비활성화

        // 사운드 사용 유무 체크
        masterVolumeToggle.onValueChanged.AddListener(OnMasterVolumeToggleChanged);
        effectVolumeToggle.onValueChanged.AddListener(OnEffectVolumeToggleChanged);
    }

    public void OnKeySettingButtonClick()
    {
        keySettings.SetActive(true);
        soundSettings.SetActive(false); // 사운드 설정 페이지 비활성화
    }

    private void OnMasterVolumeToggleChanged(bool isOn)
    {
        masterVolumeSlider.interactable = isOn; // 슬라이더 활성화/비활성화
        if (!isOn)
        {
            masterVolumeSlider.value = 0; // 체크 해제 시 0으로 설정
        }
    }

    private void OnEffectVolumeToggleChanged(bool isOn)
    {
        effectVolumeSlider.interactable = isOn; // 슬라이더 활성화/비활성화
        if (!isOn)
        {
            effectVolumeSlider.value = 0; // 체크 해제 시 0으로 설정
        }
    }

    public void OnSaveButtonClick()
    {
        // 설정 저장 처리
        masterVolume = masterVolumeToggle.isOn ? masterVolumeSlider.value : 0;
        effectVolume = effectVolumeToggle.isOn ? effectVolumeSlider.value : 0;

        Debug.Log("Settings Saved");
    }
}
