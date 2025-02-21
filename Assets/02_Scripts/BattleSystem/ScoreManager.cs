using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using VInspector.Libs;

public class ScoreManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static ScoreManager Instance { get; private set; } = null;

    Dictionary<int, PlayerScoreEntryData> playerScoreEntryDict = new Dictionary<int, PlayerScoreEntryData>();

    int killScoreReward = 1;

    bool isFinalMinute = false;
    bool isGameRunning = true;

    private void Awake()
    {
        Instance = this;

        Init();
    }
    public void Init()
    {
        playerScoreEntryDict.Clear();

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // ActorNumber와 매칭해서 플레이어의 기록 초기화
            playerScoreEntryDict[actorNumber] = new PlayerScoreEntryData(0, 0, 0, 0, actorNumber);
        }
        isFinalMinute = false;
        isGameRunning = true;
        killScoreReward = 1;
    }
    public void AddScore(int killerActorNumber, int victimActorNumber, int bonusReward = 0)
    {
        if (!isGameRunning) return;

        playerScoreEntryDict[victimActorNumber].SetDeath(playerScoreEntryDict[victimActorNumber].Death + 1);

        // 피해자의 데스 카운트 갱신
        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.UpdateDeathCount, // 데스 카운트 갱신 코드로써 전달
            new object[] { victimActorNumber, playerScoreEntryDict[victimActorNumber].Death },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);

        // 자살이 아닌 경우
        if (killerActorNumber != victimActorNumber)
        {
            playerScoreEntryDict[killerActorNumber].SetScore(
            playerScoreEntryDict[killerActorNumber].Score +
            (killScoreReward + bonusReward) *
            (isFinalMinute ? 2 : 1)
            );


            playerScoreEntryDict[killerActorNumber].SetKill(playerScoreEntryDict[killerActorNumber].Kill + 1);

            // 킬러의 점수 갱신
            PhotonNetwork.RaiseEvent(
                (byte)RaiseEventCode.UpdateScore, // 점수 갱신 코드로써 전달
                new object[] { killerActorNumber, playerScoreEntryDict[killerActorNumber].Score },
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);

            // 킬러의 킬 카운트 갱신
            PhotonNetwork.RaiseEvent(
                (byte)RaiseEventCode.UpdateKillCount, // 킬 카운트 갱신 코드로써 전달
                new object[] { killerActorNumber, playerScoreEntryDict[killerActorNumber].Kill },
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);

            HasRankingChanged(killerActorNumber);
        }

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.SaveData,
            playerScoreEntryDict.Values.ToArray(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable);
    }
    void HasRankingChanged(int actorNumber)
    {
        var sortedScores = new List<KeyValuePair<int, PlayerScoreEntryData>>(playerScoreEntryDict);

        sortedScores.Sort((x, y) => y.Value.Score.CompareTo(x.Value.Score));

        int rank = sortedScores.FindIndex(entry => entry.Key == actorNumber) + 1;

        if (rank < 4)
        {
            List<int[]> scoreList = new List<int[]>();

            rank = 1;

            scoreList.Add(new int[3] { sortedScores[0].Key, rank, sortedScores[0].Value.Score });

            for (int i = 1; i < Mathf.Min(2, PhotonNetwork.CurrentRoom.PlayerCount); i++)
            {
                if (sortedScores[i].Value.Score == sortedScores[i - 1].Value.Score)
                {
                    rank = i + 1;
                }
                scoreList.Add(new int[3] { sortedScores[i].Key, rank, sortedScores[i].Value.Score });
            }

            PhotonNetwork.RaiseEvent(
                (byte)RaiseEventCode.UpdateRealtime,
                scoreList.ToArray(),
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);
        }
    }
    public void SetIsFinalMinute(bool state)
    {
        isFinalMinute = state;
    }
    public void EndGame()
    {
        isGameRunning = false;

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.UpdateRank,
            GetScoreList(),
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }
    int[][] GetScoreList()
    {
        var sortedScores = new List<KeyValuePair<int, PlayerScoreEntryData>>(playerScoreEntryDict);

        sortedScores.Sort((x, y) => y.Value.Score.CompareTo(x.Value.Score));

        List<int[]> scoreList = new List<int[]>();

        int rank = 1;

        scoreList.Add(new int[2]{sortedScores[0].Key, rank});

        for (int i = 1; i < sortedScores.Count; i++)
        {
            if (sortedScores[i].Value.Score != sortedScores[i - 1].Value.Score)
            {
                rank = i + 1;
            }

            scoreList.Add(new int[2] { sortedScores[i].Key, rank });
        }

        return scoreList.ToArray();
    }
    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.SaveData:
                SaveData(photonEvent); break;
        }
    }
    void SaveData(EventData photonEvent)
    {
        PlayerScoreEntryData[] playerScoreEntryDatas = (PlayerScoreEntryData[])photonEvent.CustomData;

        foreach (PlayerScoreEntryData playerScoreEntryData in  playerScoreEntryDatas)

            playerScoreEntryDict[playerScoreEntryData.ActorNumber] = playerScoreEntryData;
    }
}

[Serializable]
public class PlayerScoreEntryData
{
    public int Kill { get; private set; }
    public int Death { get; private set; }
    public int Assist { get; private set; }
    public int Score { get; private set; }
    public int ActorNumber { get; private set; }

    public PlayerScoreEntryData(int kill, int death, int assist, int score, int actorNumber)
    {
        Kill = kill;
        Death = death;
        Assist = assist;
        Score = score;
        ActorNumber = actorNumber;
    }
    public void SetKill(int kill) => Kill = kill;
    public void SetDeath(int death) => Death = death;
    public void SetAssist(int assist) => Assist = assist;
    public void SetScore(int score) => Score = score;
}
//