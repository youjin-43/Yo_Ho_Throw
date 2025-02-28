using System.Collections.Generic;
using UnityEngine;

public class UI_RealtimeScoreboard : UI_Base
{
    #region VARIABLES
    List<RealtimePlayerScoreEntry> _scoreEntries = new List<RealtimePlayerScoreEntry>();
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
        //foreach (Transform child in transform)
        //{
        //    child.gameObject.SetActive(false);
        //}
    }

    public override void ResetUI()
    {

    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        for (int i = 0; i < 3; ++i)
        {
            _scoreEntries.Add(transform.GetChild(i).GetComponent<RealtimePlayerScoreEntry>());
        }
    }
    #endregion





    #region FUNCTION
    public RealtimePlayerScoreEntry InitRealtimeScoreboard(int order, string nickName)
    {
        order = Mathf.Clamp(order, 0, 2);

        _scoreEntries[order].Init(nickName);

        return _scoreEntries[order];
    }

    public void UpdateRealtimeScoreboardData(int order, string nickName, int rank, int score)
    {
        _scoreEntries[order].SetNickName(nickName);
        _scoreEntries[order].SetRank    (rank);
        _scoreEntries[order].SetScore   (score);
    }
    #endregion
}
