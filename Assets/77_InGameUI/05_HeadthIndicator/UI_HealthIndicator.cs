using UnityEngine;
using UnityEngine.UI;

public class UI_HealthIndicator : UI_Base
{
    #region VARIABLES
    private Image _gaugeImage;
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
        _gaugeImage.fillAmount = 1f;
        currentHealth = 3;
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _gaugeImage = transform.GetChild(2).GetComponent<Image>();    
    }
    #endregion





    #region FUNCTION
    public void OnHealthChanged(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, 3);

        _gaugeImage.fillAmount = 0.01667f * Mathf.Pow(currentHealth, 3) - 0.1f * Mathf.Pow(currentHealth, 2) + 0.4833f * currentHealth;
    }
    #endregion





    #region DEBUG
    int currentHealth = 3;

    public void OnHealthChangedDebug(int healthDelta)
    {
        currentHealth += healthDelta;

        currentHealth = Mathf.Clamp(currentHealth, 0, 3);

        _gaugeImage.fillAmount = 0.01667f * Mathf.Pow(currentHealth, 3) - 0.1f * Mathf.Pow(currentHealth, 2) + 0.4833f * currentHealth;
    }
    public void SetHealth(int health)
    {
        currentHealth = health;
        _gaugeImage.fillAmount = 0.01667f * Mathf.Pow(currentHealth, 3) - 0.1f * Mathf.Pow(currentHealth, 2) + 0.4833f * currentHealth;
    }
    #endregion
}
