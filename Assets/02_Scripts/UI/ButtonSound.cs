using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    // 버튼 클릭 시 이펙트 소리
    //public AudioClip clickSound; // 클릭 소리로 사용할 AudioClip
    //public AudioClip toggleSound; // 토글 소리로 사용할 AudioClip

    void Start()
    {
        // AudioSource 컴포넌트를 추가
        //audioSource = gameObject.AddComponent<AudioSource>();
        //audioSource.clip = clickSound; // AudioClip을 AudioSource에 할당

        // 버튼에 리스너 추가 (시작 시 활성화 되어있는 애들)
        RegisterButtonSounds();
    }

    public void RegisterButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None); // 모든 버튼 검색

        foreach (Button button in buttons)
        {
            if (button != null) 
            {
                button.onClick.RemoveAllListeners(); // 기존에 달려있던 리스너 제거
                button.onClick.AddListener(PlayButtonSound); // 리스너 추가
            }
        }
    }

    public void RegisterToggleSounds()
    {
        Toggle[] toggles= FindObjectsByType<Toggle>(FindObjectsSortMode.None); // 모든 토글 검색

        foreach (Toggle toggle in toggles)
        {
            if (toggle != null) 
            {
                toggle.onValueChanged.AddListener(delegate { PlayToggleSound(toggle); });
            }
        }
    }

    void PlayButtonSound()
    {
        //audioSource.PlayOneShot(clickSound); // 버튼 클릭 시 소리 재생
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick);
    }

    void PlayToggleSound(Toggle toggle)
    {
        //audioSource.PlayOneShot(toggleSound); // 토글 클릭 시 소리 재생
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIToggle);
    }
}
