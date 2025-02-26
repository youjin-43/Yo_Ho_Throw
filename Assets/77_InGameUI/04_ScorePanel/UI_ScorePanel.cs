using System.Collections.Generic;
using UnityEngine;

public class UI_ScorePanel : MonoBehaviour
{
    // int : player Å°°ª(ActorNumber)
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

    public void InitScorePanel(PlayerScoreEntry playerScoreEntryPrefab, int actorNumber, string nickName)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, transform.GetChild(0));

        entry.Init(nickName);

        _playerScoreEntries[actorNumber] = entry;
    }
}
