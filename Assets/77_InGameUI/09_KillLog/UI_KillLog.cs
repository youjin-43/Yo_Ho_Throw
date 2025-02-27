using System.Collections.Generic;
using UnityEngine;

public class UI_KillLog : UI_Base
{
    #region VARIABLES
    private Queue<KillLogPanel> _killLogQueue = new Queue<KillLogPanel>();
    private KillLogPanel _back = null;

    private float _padding = 60f;
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
        // gameObject.SetActive(false);
    }

    public override void ResetUI()
    {
    }
    #endregion





    #region FUNCTION
    public void AddKillLog(KillLogPanel killLogPanelPrefab, string killerNickname, string victimNickname)
    {
        KillLogPanel killLogPanel = null;

        if(_killLogQueue.Count > 0)
        {
            killLogPanel = _killLogQueue.Dequeue();
            killLogPanel.gameObject.SetActive(true);
        }
        else
        {
            killLogPanel = Instantiate(killLogPanelPrefab, transform);
        }

        killLogPanel.transform.localPosition = Vector3.zero;
        killLogPanel.SetText(killerNickname, victimNickname);
        killLogPanel.SetBack(_back);

        _back?.SetFront(killLogPanel);
        _back?.MoveUp(killLogPanel.transform.position.y + _padding, _padding);
        _back = killLogPanel;
    }

    public void ReturnPanel(KillLogPanel panel)
    {
        panel.ClearFrontBack();

        if(_back != null)
        {
            if(_back.Equals(panel))
            {
                _back = null;
            }
        }
        _killLogQueue.Enqueue(panel);

        panel.gameObject.SetActive(false);
    }
    #endregion
}
