using System.Collections.Generic;
using UnityEngine;

public class UI_ScoreHUD : MonoBehaviour
{
    List<RealtimePlayerScoreEntry> _scoreEntries = new List<RealtimePlayerScoreEntry>();

    void Awake()
    {
        for(int i = 0; i < 3; ++i)
        {
            _scoreEntries.Add(transform.GetChild(i).GetComponent<RealtimePlayerScoreEntry>());
        }
    }

    public void ResetUI()
    {

    }

    public void SetScoreHUDData(int order, string nickName, int rank, int score)
    {
        _scoreEntries[order].SetNickName(nickName);
        _scoreEntries[order].SetRank    (rank);
        _scoreEntries[order].SetScore   (score);
    }
}
