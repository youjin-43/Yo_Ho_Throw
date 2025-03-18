using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject blurPage;

    [SerializeField] GameObject logInPanel;
    [SerializeField] GameObject manualPanel;
    [SerializeField] GameObject settingPanel;

    [SerializeField] ButtonSound buttonSound;

    public void OnLogInButtonClick() // 로그인 버튼 클릭 시
    {
        if(logInPanel != null) logInPanel.SetActive(true);
        buttonSound.RegisterButtonSounds();
    }

    public void OnManualButtonClick() // 매뉴얼 버튼 클릭 시
    {
        if (manualPanel != null) manualPanel.SetActive(true);
        buttonSound.RegisterButtonSounds();
    }

    public void OnStartButtonClick()
    {
        ////logInPanel.SetActive(false);
        //gameObject.SetActive(false); // 타이틀 페이지 비활성화
        //blurPage.SetActive(true);  // 룸 리스트 페이지 활성화
        buttonSound.RegisterButtonSounds();
    }

    public void OnLogInCloseButtonClick()
    {
        logInPanel.SetActive(false);
    }
    public void OnManualCloseButtonClick()
    {
        manualPanel.SetActive(false);
    }

    public void OnSettingButtonClick()
    {
        settingPanel.SetActive(true);
    }

    public void OnGameOverButtonClick()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false; 
    }
}
