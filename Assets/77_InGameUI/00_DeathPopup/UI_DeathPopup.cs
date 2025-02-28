using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DeathPopup : UI_Base
{
    #region VARIABLES
    private Image           _gauge;
    private TextMeshProUGUI _timerText;
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
        _gauge.fillAmount = 0;
        _timerText.text = "0.000";
    }
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        _gauge     = transform.GetChild(0).GetChild(2).GetComponent<Image>();
        _timerText = transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>();
    }
    #endregion





    #region FUNCTION
    public void Death(float respawnTime)
    {
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        InGameUIManager.Instance.OffAllUI(_name);

        StartCoroutine(DeathPopupActive(respawnTime));   
    }

    private IEnumerator DeathPopupActive(float respawnTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < respawnTime)
        {
            elapsedTime += Time.deltaTime;

            _gauge.fillAmount = elapsedTime / respawnTime;
            _timerText.text = (respawnTime - elapsedTime).ToString("0.000");

            yield return null;
        }

        InGameUIManager.Instance.OnAllUI();
        InGameUIManager.Instance.ResetAllUI();
        
        gameObject.SetActive(false);
    }
    #endregion
}
