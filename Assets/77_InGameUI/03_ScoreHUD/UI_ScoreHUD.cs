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

    public void ToggleScorePanelUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void InitScoreHUD(int order, string nickName)
    {
        order = Mathf.Clamp(order, 0, 2);

        _scoreEntries[order].Init(nickName);
    }

    public void UpdateScoreHUDData(int order, string nickName, int rank, int score)
    {
        _scoreEntries[order].SetNickName(nickName);
        _scoreEntries[order].SetRank    (rank);
        _scoreEntries[order].SetScore   (score);
    }
}
