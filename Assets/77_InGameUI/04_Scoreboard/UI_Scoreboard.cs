using System.Collections.Generic;
using UnityEngine;

public class UI_Scoreboard : UI_Base
{
    #region VARIABLES
    // int : player Å°°ª(ActorNumber)
    Dictionary<int, PlayerScoreEntry> _playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();
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
        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {
    }
    #endregion





    #region FUNCTION
    public void ShowScoreboardUI(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public PlayerScoreEntry InitScoreboard(PlayerScoreEntry playerScoreEntryPrefab, int actorNumber, string nickName)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, transform.GetChild(0));

        entry.Init(nickName);

        _playerScoreEntries[actorNumber] = entry;

        return entry;
    }

    public void UpdateScoreboardData_DeathCount(int actorNumber, int deathCount)
    {
        _playerScoreEntries[actorNumber].SetDeathCount(deathCount);
    }

    public void UpdateScoreboardData_Score(int actorNumber, int score)
    {
        _playerScoreEntries[actorNumber].SetScore(score);
    }

    public void UpdateScoreboardData_KillCount(int actorNumber, int killCount)
    {
        _playerScoreEntries[actorNumber].SetKillCount(killCount);
    }

    public void ResetScoreboard()
    {
        foreach(var entry in _playerScoreEntries.Values)
        {
            entry.ResetPlayerScoreEntry();
        }
    }
    #endregion
}
