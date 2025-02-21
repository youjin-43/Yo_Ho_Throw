using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleUIController : MonoBehaviour, IOnEventCallback
{
    public static BattleUIController Instance { get; private set; } = null;
    
    [SerializeField] PlayerScoreEntry playerScoreEntryPrefab;

    [SerializeField] GameObject scoreboardPanel;

    [SerializeField] Transform scoreEntryParent;

    Dictionary<int, PlayerScoreEntry> playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

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
        for (int i = 0; i < 6; i++) playerScoreEntries[0] = InstantiatePlayerScoreEntry(null);
    }
    private void Update()
    {
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
        UpdatePlayerScoreEntry(photonEvent, (RaiseEventCode)photonEvent.Code);

        //switch ((RaiseEventCode)photonEvent.Code)
        //{
        //    case RaiseEventCode.UpdateKillCount:    // 킬 변경 이벤트
        //        UpdateKillCount(photonEvent); break;

        //    case RaiseEventCode.UpdateDeathCount:   // 데스 변경 이벤트
        //        UpdateDeathCount(photonEvent); break;

        //    case RaiseEventCode.UpdateAssistCount:  // 어시스트 변경 이벤트
        //        UpdateDeathCount(photonEvent); break;

        //    case RaiseEventCode.UpdateScore:        // 점수 변경 이벤트
        //        UpdateScore(photonEvent); break;
        //}
    }
    void UpdatePlayerScoreEntry(EventData photonEvent, RaiseEventCode raiseEventCode)
    {

    }
    void UpdateKillCount(EventData photonEvent)
    {
        int actorNumber = ((int[])photonEvent.CustomData)[0];
        int killCount = ((int[])photonEvent.CustomData)[1];

        playerScoreEntries[actorNumber].SetKillCount(killCount);
    }
    void UpdateDeathCount(EventData photonEvent)
    {
        int actorNumber = ((int[])photonEvent.CustomData)[0];
        int killCount = ((int[])photonEvent.CustomData)[1];

        playerScoreEntries[actorNumber].SetKillCount(killCount);
    }
    void UpdateAssistCount(EventData photonEvent)
    {
        int actorNumber = ((int[])photonEvent.CustomData)[0];
        int killCount = ((int[])photonEvent.CustomData)[1];

        playerScoreEntries[actorNumber].SetKillCount(killCount);
    }
    void UpdateScore(EventData photonEvent)
    {
        int actorNumber = ((int[])photonEvent.CustomData)[0];
        int killCount = ((int[])photonEvent.CustomData)[1];

        playerScoreEntries[actorNumber].SetKillCount(killCount);
    }
    PlayerScoreEntry InstantiatePlayerScoreEntry(Player player)
    {
        PlayerScoreEntry entry = Instantiate(playerScoreEntryPrefab, scoreEntryParent);

        entry.Init(player);

        return entry;
    }
}