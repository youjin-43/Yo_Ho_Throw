using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_OnDamage : UI_Base
{
    #region VARIABLES
    private Image _damageEffect;
    private Color _effectColor;
    private Color _defaultColor;
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
        _damageEffect = transform.GetChild(0).GetComponent<Image>();

        _effectColor  = new Color(1, 0, 0, 0.3f);
        _defaultColor = new Color(0, 0, 0, 0);
    }
    #endregion





    #region FUNCTION
    public void OnDamage()
    {
        StopAllCoroutines();

        // 대미지를 입었다면

        // 1. 빨간 반투명 이미지가 화면을 덮어야됨
        _damageEffect.color = _effectColor;


        // 2. 서서히 연해짐
        StartCoroutine(DamageEffect());

        // 그런데 알파값이 빠지는 도중에도 대미지는 입을 수 있음
        // 그러면 다시 최초 반투명 색으로 돌아가서 다시 알파를 빼야될 것 같은데

        // 여기에 들어올 때 코루틴을 끄면 되나?
    }

    private IEnumerator DamageEffect()
    {
        // 2-1. 서서히 연해지려면 알파를 빼면 되잖아
        float alpha = _effectColor.a;

        while (_damageEffect.color.a > 0)
        {
            alpha -= Time.deltaTime * 0.5f;

            _damageEffect.color = new Color(1, 0, 0, alpha);

            yield return null;
        }

        // 여기까지 왔다면 알파가 0까지 내려간거임
        // 그러면 이미지를 투명하게 바꾸면 될듯
        _damageEffect.color = _defaultColor;
    }
    #endregion
}
