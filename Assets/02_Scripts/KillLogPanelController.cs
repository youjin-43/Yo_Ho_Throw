using System.Collections.Generic;
using UnityEngine;

public class KillLogPanelController : MonoBehaviour
{
    static KillLogPanelController instance = null;

    [SerializeField] KillLogPanel killLogPanelPrefab;

    [SerializeField] RectTransform killLogPanelParent;

    [SerializeField] float padding = 30f;

    Queue<KillLogPanel> queue;

    KillLogPanel back = null;

    private void Awake()
    {
        instance = this;
    }
    public static void AddKillLog(string killerNickname, string victimNickname)
    {
        KillLogPanel killLogPanel = null;

        if (instance.queue.Count > 0) 
        {
            killLogPanel = instance.queue.Dequeue();

            killLogPanel.gameObject.SetActive(true);
        }

        else killLogPanel = Instantiate(instance.killLogPanelPrefab, instance.killLogPanelParent);

        killLogPanel.transform.localPosition = Vector3.zero;

        killLogPanel.SetText(killerNickname, victimNickname);

        killLogPanel.SetBack(instance.back);

        instance.back?.SetFront(killLogPanel);

        instance.back?.MoveUp(killLogPanel.transform.position.y + instance.padding, instance.padding);

        instance.back = killLogPanel;
    }
    public static void ReturnPanel(KillLogPanel panel)
    {
        panel.ClearFrontBack();

        if (instance.back != null)
        {
            if (instance.back.Equals(panel))
            {
                instance.back = null;
            }
        }
        instance.queue.Enqueue(panel);

        panel.gameObject.SetActive(false);
    }
}
