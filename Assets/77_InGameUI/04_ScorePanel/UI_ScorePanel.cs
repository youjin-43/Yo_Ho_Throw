using System.Collections.Generic;
using UnityEngine;

public class UI_ScorePanel : MonoBehaviour
{
    // int : player Å°°ª
    Dictionary<int, PlayerScoreEntry> _playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

    public void ResetUI()
    {

    }

    public void ToggleScorePanelUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ShowScorePanelUI(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void CreateScorePanelList(int actorNumber, PlayerScoreEntry entry)
    {
        //_playerScoreEntries[actorNumber] = ins
    }
}
