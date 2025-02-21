using UnityEngine;
using UnityEngine.UI;

public class UI_HealthIndicator : MonoBehaviour
{
    private Image _gaugeImage;

    void Awake()
    {
        _gaugeImage = transform.GetChild(2).GetComponent<Image>();    
    }

    public void ResetUI()
    {
        _gaugeImage.fillAmount = 1f;
    }

    public void OnHealthChanged(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, 3);

        _gaugeImage.fillAmount = 0.01667f * Mathf.Pow(currentHealth, 3) - 0.1f * Mathf.Pow(currentHealth, 2) + 0.4833f * currentHealth;
    }


    #region DEBUG

    int currentHealth = 3;

    public void OnHealthChangedDebug(int healthDelta)
    {
        currentHealth += healthDelta;

        currentHealth = Mathf.Clamp(currentHealth, 0, 3);

        _gaugeImage.fillAmount = 0.01667f * Mathf.Pow(currentHealth, 3) - 0.1f * Mathf.Pow(currentHealth, 2) + 0.4833f * currentHealth;
    }

    #endregion
}
