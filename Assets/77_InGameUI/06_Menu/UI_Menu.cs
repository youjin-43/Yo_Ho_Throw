using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Menu : MonoBehaviour
{
    public void ResetUI()
    {

    }

    public void ToggleMenuUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

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
}
