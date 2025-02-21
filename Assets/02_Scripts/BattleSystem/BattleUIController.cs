using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour, IOnEventCallback
{
    public static BattleUIController Instance { get; private set; } = null;
    
    [SerializeField] PlayerScoreEntry playerScoreEntryPrefab;

    [SerializeField] GameObject scoreboardPanel;

    [SerializeField] Transform scoreEntryParent;

    [SerializeField] Button titleButton;
    [SerializeField] Button endGameButton;
    [SerializeField] Button lobbyButton;

    [SerializeField] TMP_Text limitTimeText;

    [SerializeField] Sprite goldMedalSprite;
    [SerializeField] Sprite silverMedalSprite;
    [SerializeField] Sprite bronzeMedalSprite;

    Dictionary<int, PlayerScoreEntry> playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

    bool isGameEnded = false;

    private void Awake()
    {
        Instance = this;

        scoreboardPanel.SetActive(false);

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // 비어있는 UI에 PlayerScoreEntry를 추가하고 Dictionary로 actorNumber와 매칭한다
            playerScoreEntries[actorNumber] = InstantiatePlayerScoreEntry(PhotonNetwork.CurrentRoom.Players[actorNumber]);
        }
        //생성 테스트
        //for (int i = 0; i < 6; i++) Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        isGameEnded = false;

        // 버튼 비활성화
        titleButton.gameObject.SetActive(false);
        endGameButton.gameObject.SetActive(false);
        lobbyButton.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isGameEnded) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowScoreboard();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }
    }
    void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);
    }
    void HideScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }
    public void OnEvent(EventData photonEvent)
    {
        RaiseEventCode raiseEventCode = (RaiseEventCode)photonEvent.Code;

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateKillCount:
            case RaiseEventCode.UpdateDeathCount:
            case RaiseEventCode.UpdateScore:
                UpdatePlayerScoreEntry(photonEvent, raiseEventCode); break;

            case RaiseEventCode.ResetScoreboard:
                ResetScoreboard(); break;

            case RaiseEventCode.UpdateRank:
                UpdateRank(photonEvent); break;
        }
    }
    void UpdateRank(EventData photonEvent)
    {
        (int, int)[] scoreList = ((int, int)[])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            playerScoreEntries[scoreList[i].Item1].transform.SetAsLastSibling();

            switch (scoreList[i].Item2)
            {
                case 1:
                    playerScoreEntries[scoreList[i].Item1].SetIconImage(goldMedalSprite); break;
                case 2:
                    playerScoreEntries[scoreList[i].Item1].SetIconImage(silverMedalSprite); break;
                case 3:
                    playerScoreEntries[scoreList[i].Item1].SetIconImage(bronzeMedalSprite); break;
            }
        }

        // 게임 종료 시의 UI 표시
        EndGame();
    }
    void UpdatePlayerScoreEntry(EventData photonEvent, RaiseEventCode raiseEventCode)
    {
        // 수정할 대상의 ActorNumber와 변경 값 가져오기
        int actorNumber = ((int[])photonEvent.CustomData)[0];
        int value = ((int[])photonEvent.CustomData)[1];

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateKillCount:
                playerScoreEntries[actorNumber].SetKillCount(value); break;

            case RaiseEventCode.UpdateDeathCount:
                playerScoreEntries[actorNumber].SetDeathCount(value); break;

            case RaiseEventCode.UpdateScore:
                playerScoreEntries[actorNumber].SetScore(value); break;
        }
    }
    public void EndGame()
    {
        isGameEnded = true;

        ShowScoreboard();

        // 버튼 활성화
        titleButton.gameObject.SetActive(true);
        endGameButton.gameObject.SetActive(true);
        lobbyButton.gameObject.SetActive(true);
    }
    void ResetScoreboard()
    {
        foreach (PlayerScoreEntry playerScoreEntry in playerScoreEntries.Values)
        {
            playerScoreEntry.ResetPlayerScoreEntry();
        }
    }
    public void SetLimitTimeText(int seconds)
    {
        limitTimeText.text = string.Format("{0}:{1:00}", seconds % 60, seconds);
    }
    PlayerScoreEntry InstantiatePlayerScoreEntry(Player player)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        entry.Init(player);

        return entry;
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}