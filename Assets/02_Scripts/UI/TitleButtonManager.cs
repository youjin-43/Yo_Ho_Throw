using UnityEngine;

public class TitleButtonManager : MonoBehaviour
{
    // 타이틀 씬에 있는 버튼 이벤트 함수

    [SerializeField] GameObject logInPanel;
    [SerializeField] GameObject manualPanel;

    public void OnLogInButtonClick() // 로그인 버튼 클릭 시
    {
        if(logInPanel != null) logInPanel.SetActive(true);
    }

    public void OnManualButtonClick() // 매뉴얼 버튼 클릭 시
    {
        if (manualPanel != null) manualPanel.SetActive(true);
    }
}
