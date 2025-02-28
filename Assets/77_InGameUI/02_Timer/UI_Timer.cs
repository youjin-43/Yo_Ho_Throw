using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Timer : UI_Base
{
    #region VARIABLES
    private TextMeshProUGUI _timerText;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
    }

    public override void On()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public override void Off()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public override void ResetUI()
    {
        StopAllCoroutines();

        _timerText.text = $"0 : 00";
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _timerText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    #endregion





    #region FUNCTION
    public void StartTimer(float time)
    {
        StartCoroutine(SetTimer(time));
    }

    private IEnumerator SetTimer(float time)
    {
        float timer = time;

        while(timer > 0)
        {
            yield return null;

            timer -= Time.deltaTime;
            _timerText.text = $"{(int)(timer / 60)} : {(int)(timer % 60):D2}";
        }
    }
    #endregion
}
