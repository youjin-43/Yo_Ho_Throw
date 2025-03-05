using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour, IOnEventCallback
{
    public static BattleUIController Instance { get; private set; } = null;

    [Header("플레이어 점수 기록 프리팹")]
    [SerializeField] PlayerScoreEntry playerScoreEntryPrefab;

    [Header("플레이어의 점수 기록 패널 부모 GameObject")]
    [SerializeField] GameObject scoreboardPanel;

    [Header("플레이어의 점수 기록 패널 부모 Transform")]
    [SerializeField] Transform scoreEntryParent;

    [Header("게임 종료 시 표시할 버튼들")]
    [SerializeField] Button titleButton;
    [SerializeField] Button lobbyButton;

    [Header("LimitTime")]
    [SerializeField] TMP_Text limitTimeText;

    [Header("순위에 따른 이미지들")]
    [SerializeField] Sprite goldMedalSprite;
    [SerializeField] Sprite silverMedalSprite;
    [SerializeField] Sprite bronzeMedalSprite;

    [Header("부활까지 남은 시간")]
    [SerializeField] TMP_Text respawnRemainingText;

    [Header("연속 처치 텍스트")]
    [SerializeField] TMP_Text comboKillText;

    [Header("전투 시작 텍스트")]
    [SerializeField] TMP_Text battleStartText;

    Dictionary<int, PlayerScoreEntry> playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

    List<RealtimePlayerScoreEntry> realtimeScoreEntry = new List<RealtimePlayerScoreEntry>();

    bool isGameRunning = true;
    bool isAlive = true;
    private void Awake()
    {
        Instance = this;

        scoreboardPanel.SetActive(false);

        int i = 0;

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // 탭 누르면 나오는 스코어보드
            {
                playerScoreEntries[actorNumber] =
                    InGameUIManager.Instance.InitScoreboard(actorNumber, PhotonNetwork.CurrentRoom.Players[actorNumber].NickName);
            }


            // 우상단에 상시 표시되는 스코어보드
            {
                // 2. InGameUI 연동
                if (i < 3)
                {
                    realtimeScoreEntry.Add(InGameUIManager.Instance.InitRealtimeScoreboard(
                            i, PhotonNetwork.CurrentRoom.Players[actorNumber].NickName));
                    realtimeScoreEntry[realtimeScoreEntry.Count - 1]._isVisible = true;
                    i++;
                }

            }
        }

        isGameRunning = true;

        // 버튼 비활성화
        titleButton.gameObject.SetActive(false);
        lobbyButton.gameObject.SetActive(false);

        comboKillText.text = string.Empty;
    }
    private void Update()
    {
        if (!isGameRunning) return;

        if (Input.GetKeyDown(KeyCode.Tab) && isAlive)
        {
            ShowScoreboard();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }
    }
    public void SetIsAlive(bool isAlive)
    {
        this.isAlive = isAlive;

        if (isAlive == false)
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

            case RaiseEventCode.UpdateRealtime:
                UpdateRealtime(photonEvent); break;
        }
    }
    void UpdateRealtime(EventData photonEvent)
    {
        int[][] scoreList = (int[][])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            // 1. 기존 로직
            {
                realtimeScoreEntry[i].gameObject.SetActive(true);

                realtimeScoreEntry[i].SetNickName(PhotonNetwork.CurrentRoom.Players[scoreList[i][0]].NickName);

                realtimeScoreEntry[i].SetRank(scoreList[i][1]);

                realtimeScoreEntry[i].SetScore(scoreList[i][2]);
            }

            // 2. InGameUI 연동
            // InGameUIManager.Instance.UpdateScoreHUDData(i, PhotonNetwork.CurrentRoom.Players[scoreList[i][0]].NickName, scoreList[i][1], scoreList[i][2]);
        }
    }
    void UpdateRank(EventData photonEvent)
    {
        int[][] scoreList = (int[][])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            playerScoreEntries[scoreList[i][0]].transform.SetAsLastSibling();

            switch (scoreList[i][1])
            {
                case 1:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(goldMedalSprite); break;
                case 2:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(silverMedalSprite); break;
                case 3:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(bronzeMedalSprite); break;
            }
        }

        // 게임 종료 시의 UI 표시
        EndGame();
    }
    void UpdatePlayerScoreEntry(EventData photonEvent, RaiseEventCode raiseEventCode)
    {
        int[] data = (int[])photonEvent.CustomData;

        // 수정할 대상의 ActorNumber와 변경 값 가져오기
        int actorNumber = data[0];
        int value = data[1];

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateDeathCount:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetDeathCount(value); break;

                // 2. InGameUI 연동
                // InGameUIManager.Instance.UpdateScoreboardData_DeathCount(actorNumber, value);

            case RaiseEventCode.UpdateScore:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetScore(value); break;

                // 2. InGameUI 연동
                // InGameUIManager.Instance.UpdateScoreboardData_Score(actorNumber, value);

            case RaiseEventCode.UpdateKillCount:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetKillCount(value); break;

                // 2. InGameUI 연동
                // InGameUIManager.Instance.UpdateScoreboardData_KillCount(actorNumber, value);
        }
    }
    public void EndGame()
    {
        isGameRunning = false;

        ShowScoreboard();

        // 버튼 활성화
        titleButton.gameObject.SetActive(true);
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
        limitTimeText.text = $"{(seconds / 60).ToString()} : {(seconds % 60).ToString("00")}";
    }
    PlayerScoreEntry InstantiatePlayerScoreEntry(Player player)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        entry.Init(player.NickName);

        return entry;
    }
    public void SetRespawnTimer(int timer)
    {
        if (timer > 0)
        {
            Debug.Log("Timer > 0");
            respawnRemainingText.text = timer.ToString();
        }
        else
        {
            Debug.Log("Else");
            respawnRemainingText.text = string.Empty;
        }
    }
    public void SetComboKill(int combo)
    {
        comboKillText.text = combo > 1 ? combo.ToString() + " Combo" : string.Empty;
    }
    public void SetBattleStartText(int count)
    {
        switch (count)
        {
            case 2: battleStartText.text = "Ready"; break;

            case 1: battleStartText.text = "Go"; break;

            case 0: StartCoroutine(HideTextAfterTimeCoroutine(battleStartText, 1f)); break;
        }
    }
    IEnumerator HideTextAfterTimeCoroutine(TMP_Text targetTextUI, float delay)
    {
        yield return new WaitForSeconds(delay);

        targetTextUI.text = string.Empty;
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}