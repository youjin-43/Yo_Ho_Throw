using System.Collections.Generic;
using UnityEngine;

public class UI_RealtimeScoreboard : UI_Base
{
    #region VARIABLES
    [SerializeField] RealtimePlayerScoreEntry[] _scoreEntries;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
    }

    public override void On()
    {
        //foreach (RealtimePlayerScoreEntry realtimePlayerScoreEntry in _scoreEntries)
        //{
        //    if (realtimePlayerScoreEntry._isVisible) realtimePlayerScoreEntry.gameObject.SetActive(true);
        //}

        gameObject.SetActive(true);
    }

    public override void Off()
    {
        //foreach (RealtimePlayerScoreEntry realtimePlayerScoreEntry in _scoreEntries)
        //{
        //    if (realtimePlayerScoreEntry._isVisible) realtimePlayerScoreEntry.gameObject.SetActive(false);
        //}

        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {

    }
    #endregion





    #region FUNCTION
    public RealtimePlayerScoreEntry InitRealtimeScoreboard(int order, string nickName)
    {
        order = Mathf.Clamp(order, 0, 2);

        _scoreEntries[order].Init(nickName);

        _scoreEntries[order].gameObject.SetActive(true);

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
