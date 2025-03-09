using UnityEngine.UI;

public class UI_GameOverPopup : UI_Base
{
    #region VARIABLES
    private Button ReturnToWaitingRoomButton;
    private Button ReturnToTitleButton;
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





    #region BEHAVIOURS
    void Awake()
    {
        ReturnToWaitingRoomButton = transform.GetChild(0).GetChild(2).GetComponent<Button>();
        ReturnToTitleButton       = transform.GetChild(0).GetChild(3).GetComponent<Button>();

        ReturnToWaitingRoomButton.onClick.AddListener(ReturnToWaitingRoom);
        ReturnToTitleButton      .onClick.AddListener(ReturnToTitle);
    }
    #endregion





    #region FUNCTION
    public void ReturnToWaitingRoom()
    {
        // GameReadyScene¿∏∑Œ
    }

    public void ReturnToTitle()
    {
        // SceneManager.LoadSceneAsync()
    }
    #endregion
}
