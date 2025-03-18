using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillLogPanel : MonoBehaviour
{
    [SerializeField] TMP_Text killerNicknameText;
    [SerializeField] TMP_Text victimNicknameText;
    [SerializeField] TMP_Text addScoreText;
    [SerializeField] Image iconImage;

    KillLogPanel front = null;
    KillLogPanel back = null;

    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void MoveUp(float positionY, float padding)
    {
        if (transform.position.y < positionY)
        {
            float distance = positionY - transform.position.y;

            transform.position += new Vector3(0, distance, 0);

            back?.MoveUp(transform.position.y + padding, padding);
        }
    }
    public void SetText(string killerNickname, string victimNickname, Sprite icon, int addScore = 1)
    {
        killerNicknameText.text = killerNickname;
        victimNicknameText.text = victimNickname;
        iconImage.sprite = icon;

        //TMP 크기 조절 (하지만 제거)
        //killerNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, killerNicknameText.preferredWidth);
        //victimNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, victimNicknameText.preferredWidth);

        canvasGroup.alpha = 0;

        addScoreText.text = "+" + addScore.ToString();

        StartCoroutine(FadeCoroutine());
        StartCoroutine(AddScoreFadeCoroutine());
    }
    public void ClearFrontBack()
    {
        if (front != null) front.back = null;
    }
    public void SetFront(KillLogPanel panel) => front = panel;
    public void SetBack(KillLogPanel panel) => back = panel;
    IEnumerator FadeCoroutine()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 5f;

            canvasGroup.alpha = alpha;

            yield return null;
        }
        alpha = 1f;

        yield return new WaitForSeconds(2f);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * 5f;

            canvasGroup.alpha = alpha;

            yield return null;
        }
        //KillLogPanelController.Instance.ReturnPanel(this);
        InGameUIManager.Instance.ReturnPanel(this);
    }
    IEnumerator AddScoreFadeCoroutine()
    {
        Color addScoreTextColor = Color.white;
        addScoreTextColor.a = 0f;
        addScoreText.color = addScoreTextColor;

        yield return new WaitForSeconds(0.3f);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            addScoreTextColor.a = t;
            addScoreText.color = addScoreTextColor;
            yield return null;
        }
    }
}
