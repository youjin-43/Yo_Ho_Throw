using UnityEngine.UI;

public class UI_Menu : UI_Base
{
    #region VARIABLES
    private Button ReturnToGameButton;
    private Button SettingButton;
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





    #region BEHAVIOUR
    void Awake()
    {
        ReturnToGameButton  = transform.GetChild(0).GetChild(2).GetComponent<Button>();
        SettingButton       = transform.GetChild(0).GetChild(3).GetComponent<Button>();
        ReturnToTitleButton = transform.GetChild(0).GetChild(4).GetComponent<Button>();

        ReturnToGameButton .onClick.AddListener(Resume);
        ReturnToGameButton .onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));

        SettingButton      .onClick.AddListener(ToggleSettingUI);
        SettingButton      .onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));

        ReturnToTitleButton.onClick.AddListener(ReturnToTitle);
        ReturnToTitleButton.onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));
    }
    #endregion





    #region FUNCTION
    public void Resume()
    {
        InGameUIManager.Instance.ToggleMenuUI();
    }

    public void ToggleSettingUI()
    {
        InGameUIManager.Instance.ToggleSettingUI();

        gameObject.SetActive(false);
    }

    public void ReturnToTitle()
    {
        // SceneManager.LoadSceneAsync()
    }
    #endregion
}
