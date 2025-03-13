using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class KillLogPanelController : MonoBehaviour
{
    public static KillLogPanelController Instance { get; private set; } = null;

    [SerializeField] Sprite[] icons;

    [SerializeField] KillLogPanel killLogPanelPrefab;

    [SerializeField] RectTransform killLogPanelParent;

    [SerializeField] float padding = 30f;

    Queue<KillLogPanel> queue = new Queue<KillLogPanel>();

    KillLogPanel back = null;

    private void Awake()
    {
        Instance = this;
    }
    public void AddKillLog(int killerActorNr, int victimActorNr)
    {
        KillLogPanel killLogPanel = null;

        if (queue.Count > 0) 
        {
            killLogPanel = queue.Dequeue();

            killLogPanel.gameObject.SetActive(true);
        }

        else killLogPanel = Instantiate(killLogPanelPrefab, killLogPanelParent);

        killLogPanel.transform.localPosition = Vector3.zero;

        killLogPanel.SetText(
            PhotonNetwork.CurrentRoom.Players[killerActorNr].NickName,
            PhotonNetwork.CurrentRoom.Players[victimActorNr].NickName,
            icons[0]);

        killLogPanel.SetBack(back);

        back?.SetFront(killLogPanel);

        back?.MoveUp(killLogPanel.transform.position.y + padding, padding);

        back = killLogPanel;
    }
    public void AddKillLog(int exitActorNr)
    {
        KillLogPanel killLogPanel = null;

        if (queue.Count > 0)
        {
            killLogPanel = queue.Dequeue();

            killLogPanel.gameObject.SetActive(true);
        }

        else killLogPanel = Instantiate(killLogPanelPrefab, killLogPanelParent);

        killLogPanel.transform.localPosition = Vector3.zero;

        // TODO 호준 : 탈주 로그 텍스트 수정 부분 (찬규)
        killLogPanel.SetText(
            PhotonNetwork.CurrentRoom.Players[exitActorNr].NickName,
            "접속종료",
            icons[1]);

        killLogPanel.SetBack(back);

        back?.SetFront(killLogPanel);

        back?.MoveUp(killLogPanel.transform.position.y + padding, padding);

        back = killLogPanel;
    }
    public void ReturnPanel(KillLogPanel panel)
    {
        panel.ClearFrontBack();

        if (back != null)
        {
            if (back.Equals(panel))
            {
                back = null;
            }
        }
        queue.Enqueue(panel);

        panel.gameObject.SetActive(false);
    }
}
