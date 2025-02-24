using System.Collections;
using TMPro;
using UnityEngine;

public class KillLogPanel : MonoBehaviour
{
    [SerializeField] TMP_Text killerNicknameText;
    [SerializeField] TMP_Text victimNicknameText;

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
    public void SetText(string killerNickname, string victimNickname)
    {
        killerNicknameText.text = killerNickname;
        victimNicknameText.text = victimNickname;

        //TMP 크기 조절 (하지만 제거)
        //killerNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, killerNicknameText.preferredWidth);
        //victimNicknameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, victimNicknameText.preferredWidth);

        canvasGroup.alpha = 0;

        StartCoroutine(FadeCoroutine());
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

        yield return new WaitForSeconds(1f);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * 5f;

            canvasGroup.alpha = alpha;

            yield return null;
        }
        KillLogPanelController.ReturnPanel(this);
    }
}
