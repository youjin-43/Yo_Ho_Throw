using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{ 
    [SerializeField] Sprite buttonImage; // 기본 버튼 이미지
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

    public Slider sensitivitySlider; // 마우스 감도 슬라이더
    public TMP_Text sensitivityText; // 감도 표시 텍스트

    [SerializeField] ButtonSound buttonSound;



    // ㅇㅎㅈ
    // 여기에 Player 프리펩을 넣고 MainUIScene에서 감도설정을 하면 모든 클라이언트가 동일한 값을 가져서
    // 민감도 값을 따로 저장해놓고 플레이어가 생성된 이후에 적용시켜주면 좋을 것 같아요
    // 민감도 값을 GameManager에 저장해놓고 꺼내서 쓰는 구조로 바꿨어요.

    //[SerializeField] PlayerController playerController;
    [SerializeField] float sensitivity = 1f;
    float clampedValue;

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

        // 감도 슬라이더의 초기값 설정 (예: 4.5)
        sensitivitySlider.value = 4.5f;
        sensitivitySlider.onValueChanged.AddListener(OnSensitivitySliderChanged);
        sensitivityText.text = sensitivitySlider.value.ToString();

        // ㅇㅎㅈ
        GameManager.Instance.StoreSensitivityValue(sensitivitySlider.value);


        masterVolume = AudioManager.Instance.bgmVolume;
        effectVolume = AudioManager.Instance.sfxVolume;

        // ㅇㅎㅈ
        // sensitivity = playerController.mouseSpeed;


        // 버튼&토글에 소리 연결
        if(buttonSound == null)
        {
            buttonSound = AudioManager.Instance.GetComponent<ButtonSound>();
        }

        buttonSound.RegisterButtonSounds();
        buttonSound.RegisterToggleSounds();
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

    private void OnSensitivitySliderChanged(float value)
    {
        // 감도 값을 0.1에서 10 사이로 제한
        clampedValue = Mathf.Clamp(value, 0.1f, 10f);


        // ㅇㅎㅈ
        // 플레이어 마우스 감도 설정 
        // playerController.SetMouseSensitivity(clampedValue);
        GameManager.Instance.StoreSensitivityValue(clampedValue);


        sensitivityText.text = clampedValue.ToString("F1"); // 소수점 한자리까지 표시
    }

    public void OnSaveButtonClick()
    {
        // 설정 저장 처리
        AudioManager.Instance.bgmVolume = masterVolumeToggle.isOn ? masterVolumeSlider.value : 0;
        AudioManager.Instance.sfxVolume = effectVolumeToggle.isOn ? effectVolumeSlider.value : 0;

        // 감도 저장
        sensitivity = clampedValue;
        Debug.Log("Settings Saved");
    }

    public void OnCloseButtonClick()
    {
        // 소리 & 키 정상화
        AudioManager.Instance.SetBgmVolume(AudioManager.Instance.bgmVolume);
        AudioManager.Instance.SetSfxVolume(AudioManager.Instance.sfxVolume);

        //playerController.SetMouseSensitivity(sensitivity);

        GameManager.Instance.StoreSensitivityValue(clampedValue);

        gameObject.SetActive(false); // 패널 비활성화
    }

    private void OnEnable()
    {
        sensitivitySlider.value = GameManager.Instance.GetStoreSensitivityValue();
        sensitivityText  .text  = GameManager.Instance.GetStoreSensitivityValue().ToString("F1");
    }
}
