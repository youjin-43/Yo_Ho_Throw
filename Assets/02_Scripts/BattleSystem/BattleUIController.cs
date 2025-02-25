using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour, IOnEventCallback
{
    public static BattleUIController Instance { get; private set; } = null;

    [Header("현재 등수 표시 점수 기록 오브젝트 배열")]
    [SerializeField] RealtimePlayerScoreEntry[] realtimeScoreEntry;

    [Header("플레이어 점수 기록 프리팹")]
    [SerializeField] PlayerScoreEntry playerScoreEntryPrefab;

    [Header("플레이어의 점수 기록 패널 부모 GameObject")]
    [SerializeField] GameObject scoreboardPanel;

    [Header("플레이어의 점수 기록 패널 부모 Transform")]
    [SerializeField] Transform scoreEntryParent;

    [Header("게임 종료 시 표시할 버튼들")]
    [SerializeField] Button titleButton;
    [SerializeField] Button endGameButton;
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

    Dictionary<int, PlayerScoreEntry> playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

    bool isGameRunning = true;

    private void Awake()
    {
        Instance = this;

        scoreboardPanel.SetActive(false);

        int i = 0;

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // 비어있는 UI에 PlayerScoreEntry를 추가하고 Dictionary로 actorNumber와 매칭한다
            playerScoreEntries[actorNumber] = InstantiatePlayerScoreEntry(PhotonNetwork.CurrentRoom.Players[actorNumber]);

            // 실시간 스코어 정보 오브젝트 초기화
            if (i < 3) realtimeScoreEntry[i++].Init(PhotonNetwork.CurrentRoom.Players[actorNumber]);
        }

        for (; i < 3; i++) // 최소 실시간 스코어 정보 오브젝트(3)보다 현재 인원이 적을 때
        {
            // 표시되고 있던 실시간 스코어 정보 오브젝트를 비활성화 한다
            realtimeScoreEntry[i].gameObject.SetActive(false);
        }

        //생성 테스트
        //for (int j = 0; j < 6; j++) Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        isGameRunning = true;

        // 버튼 비활성화
        titleButton.gameObject.SetActive(false);
        endGameButton.gameObject.SetActive(false);
        lobbyButton.gameObject.SetActive(false);

        comboKillText.text = string.Empty;
    }
    private void Update()
    {
        if (!isGameRunning) return;

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

            case RaiseEventCode.UpdateRealtime:
                UpdateRealtime(photonEvent); break;
        }
    }
    void UpdateRealtime(EventData photonEvent)
    {
        int[][] scoreList = (int[][])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            realtimeScoreEntry[i].gameObject.SetActive(true);

            realtimeScoreEntry[i].SetNickName(PhotonNetwork.CurrentRoom.Players[scoreList[i][0]].NickName);

            realtimeScoreEntry[i].SetRank(scoreList[i][1]);

            realtimeScoreEntry[i].SetScore(scoreList[i][2]);
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
        Debug.Log($"CustomData Type: {photonEvent.CustomData.GetType()}");

        Debug.Log("이벤트 종류 : " + raiseEventCode.ToString());

        int[] data = (int[])photonEvent.CustomData;

        // 수정할 대상의 ActorNumber와 변경 값 가져오기
        int actorNumber = data[0];
        int value = data[1];

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateDeathCount:
                playerScoreEntries[actorNumber].SetDeathCount(value); break;

            case RaiseEventCode.UpdateScore:
                playerScoreEntries[actorNumber].SetScore(value); break;

            case RaiseEventCode.UpdateKillCount:
                playerScoreEntries[actorNumber].SetKillCount(value); break;
        }
    }
    public void EndGame()
    {
        isGameRunning = false;

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
        limitTimeText.text = $"{(seconds / 60).ToString()} : {(seconds % 60).ToString("00")}";
    }
    PlayerScoreEntry InstantiatePlayerScoreEntry(Player player)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        entry.Init(player);

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
        comboKillText.text = combo == 0 ? string.Empty : combo.ToString() + " Combo";
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}