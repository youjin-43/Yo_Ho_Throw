using UnityEngine;

public class RoomListManager : MonoBehaviour
{
    [SerializeField] GameObject titlePage; // 타이틀 페이지

    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject createRoomPanel;

    [SerializeField] ButtonSound buttonSound;

    private void Awake()
    {
        // 버튼에 사운드 연결
        buttonSound.RegisterButtonSounds();
    }
    public void OnReloadButtonClick()
    {

    }
    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false); // 블러페이지 비활성화
        titlePage.SetActive(true);
    }
    public void OnGameOverButtonClick()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OnSettingButtonClick()
    {
        settingPanel.SetActive(true);
    }

    public void OnCreateRoomButtonClick()
    {
        createRoomPanel.SetActive(true);
    }
}
