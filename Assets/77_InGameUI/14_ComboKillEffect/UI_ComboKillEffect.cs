using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ComboKillEffect : UI_Base
{
    #region VARIABLES
    private string          _defaultText;
    private TextMeshProUGUI _combokillText_Out;
    private TextMeshProUGUI _combokillText_In;

    private Image _background;
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
        _defaultText = " 명 연속 처치";
        
        _combokillText_Out = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _combokillText_In  = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        _background = GetComponent<Image>();
    }
    #endregion





    #region FUNCTION
    public void SetCombokill(int combo)
    {
        gameObject.SetActive(true);

        StopAllCoroutines();

        StartCoroutine(StartEffect(combo));
    }

    private IEnumerator StartEffect(int num)
    {
        _combokillText_Out.text = num.ToString() + _defaultText;
        _combokillText_In .text = num.ToString() + _defaultText;

        _combokillText_Out.color = new Color(0.3f, 1, 1, 1);
        _combokillText_In.color = new Color(0, 0.6f, 0.6f, 1);

        float alpha = 0;

        float dx = 0;

        while(dx < 1.5)
        {
            dx += Time.deltaTime;

            if(dx < 0.5)
            {
                alpha = -2 * dx * dx + 2 * dx;
            }
            else if(dx < 1)
            {
                alpha = 0.5f;
            }
            else
            {
                alpha = -2 * dx * dx + 4 * dx - 1.5f;

                _combokillText_Out.color = new Color(0.3f, 1, 1, alpha * 2);
                _combokillText_In .color = new Color(0, 0.6f, 0.6f, alpha * 2);
            }

            _background.color = new Color(0.25f, 0.9f, 1, alpha);

            yield return null;
        }

        _background.color = new Color(0.25f, 0.9f, 1, 0);
        gameObject.SetActive(false);
    }
    #endregion
}
