using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_SkillIndicator : MonoBehaviour
{
    // Cooldown Effect Sprite
    private Image _skill_Shift_CooldownEffect;
    private Image _skill_LClick_CooldownEffect;
    private Image _skill_RClick_CooldownEffect;

    void Awake()
    {
        // Cooldown Effect
        _skill_Shift_CooldownEffect  = transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
        _skill_LClick_CooldownEffect = transform.GetChild(1).transform.GetChild(1).GetComponent<Image>();
        _skill_RClick_CooldownEffect = transform.GetChild(2).transform.GetChild(1).GetComponent<Image>();

        _skill_Shift_CooldownEffect.gameObject.SetActive(false);
        _skill_LClick_CooldownEffect.gameObject.SetActive(false);
        _skill_RClick_CooldownEffect.gameObject.SetActive(false);
    }

    public void StartCooldownEffect(KeyCode key, float cooldownTime)
    {
        StartCoroutine(CooldownEffect(_skill_Shift_CooldownEffect, cooldownTime));
    }

    public void StartCooldownEffect(int button, float cooldownTime)
    {
        // 좌클릭
        if (button == 0)
        {
            StartCoroutine(CooldownEffect(_skill_LClick_CooldownEffect, cooldownTime));
        }
        // 우클릭
        else if (button == 1)
        {
            StartCoroutine(CooldownEffect(_skill_RClick_CooldownEffect, cooldownTime));
        }
    }

    private IEnumerator CooldownEffect(Image coolDownImage, float cooldownTime)
    {
        coolDownImage.gameObject.SetActive(true);
        coolDownImage.fillAmount = 1f;

        float elapsedTime = 0f;

        while(elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            coolDownImage.fillAmount = 1f - (elapsedTime / cooldownTime);
            yield return null;
        }

        coolDownImage.gameObject.SetActive(false);
    }
}
