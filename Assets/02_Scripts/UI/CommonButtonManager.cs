using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonButtonManager : MonoBehaviour
{
    // 공용 버튼

    public GameObject settingPanel; // 설정 패널

    public void OnCloseButtonClick() // X버튼 클릭 시
     {
        ClosablePanel panel = GetComponentInParent<ClosablePanel>();
        Debug.Log(panel);

        if(panel != null) panel.ClosePanel();
    }

    public void OnSettingButtonClick() // 세팅 버튼 클릭 시
    {
        if (settingPanel != null) settingPanel.SetActive(true); // 설정 패널 활성화
    }

    public void OnSceneChangeButtonClick(string sceneName) // 씬 변경하는 버튼(ex.나가기or시작) 클릭 시
    {
        SceneManager.LoadScene(sceneName); // 씬 이동
    }
}
