using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    private TextMeshProUGUI _timerText;

    void Awake()
    {
        _timerText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void ResetUI()
    {
        StopAllCoroutines();

        _timerText.text = $"0 : 00";
    }

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
}
