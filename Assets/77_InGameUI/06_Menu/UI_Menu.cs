using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Menu : UI_Base
{
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
