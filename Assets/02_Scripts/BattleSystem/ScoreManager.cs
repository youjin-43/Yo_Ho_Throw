using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
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
            playerScoreEntryDict[actorNumber] = new PlayerScoreEntryData(0, 0, 0, 0);
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
            new int[] { victimActorNumber, playerScoreEntryDict[victimActorNumber].Death },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);

        // 자살일 경우 여기서 반환 (데스 카운트에 대한 처리까지만 진행)
        if (killerActorNumber == victimActorNumber) return;


        playerScoreEntryDict[killerActorNumber].SetScore(
            playerScoreEntryDict[killerActorNumber].Score +
            (killScoreReward + bonusReward) *
            (isFinalMinute ? 2 : 1)
            );


        playerScoreEntryDict[killerActorNumber].SetKill(playerScoreEntryDict[killerActorNumber].Kill + 1);

        // 킬러의 점수 갱신
        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.UpdateScore, // 점수 갱신 코드로써 전달
            new int[] { killerActorNumber, playerScoreEntryDict[killerActorNumber].Score },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);

        // 킬러의 킬 카운트 갱신
        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.UpdateKillCount, // 킬 카운트 갱신 코드로써 전달
            new int[] { killerActorNumber, playerScoreEntryDict[killerActorNumber].Kill },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);

        HasRankingChanged(killerActorNumber);
    }
    void HasRankingChanged(int actorNumber)
    {
        var sortedScores = new List<KeyValuePair<int, PlayerScoreEntryData>>(playerScoreEntryDict);

        sortedScores.Sort((x, y) => y.Value.Score.CompareTo(x.Value.Score));

        int rank = sortedScores.FindIndex(entry => entry.Key == actorNumber) + 1;

        if (rank < 4)
        {
            List<(int, int)> scoreList = new List<(int, int)>();

            rank = 1;

            scoreList.Add((sortedScores[0].Key, rank));

            for (int i = 1; i < Mathf.Min(2, PhotonNetwork.CurrentRoom.PlayerCount); i++)
            {
                if (sortedScores[i].Value.Score == sortedScores[i - 1].Value.Score)
                {
                    scoreList.Add((sortedScores[i].Key, rank));
                }
                else
                {
                    rank = i + 1;
                    scoreList.Add((sortedScores[i].Key, rank));
                }
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
            (byte)RaiseEventCode.UpdateScore,
            GetScoreList(),
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }
    (int, int)[] GetScoreList()
    { 
        var sortedScores = new List<KeyValuePair<int, PlayerScoreEntryData>>(playerScoreEntryDict);

        sortedScores.Sort((x, y) => y.Value.Score.CompareTo(x.Value.Score));

        List<(int, int)> scoreList = new List<(int, int)>();

        int rank = 1;

        scoreList.Add((sortedScores[0].Key, rank));

        for (int i = 1; i < sortedScores.Count; i++)
        {
            if (sortedScores[i].Value.Score == sortedScores[i - 1].Value.Score)
            {
                scoreList.Add((sortedScores[i].Key, rank));
            }
            else
            {
                rank = i + 1; 
                scoreList.Add((sortedScores[i].Key, rank));
            }
        }

        return scoreList.ToArray();
    }
}

public struct PlayerScoreEntryData
{
    public int Kill { get; private set; }
    public int Death { get; private set; }
    public int Assist { get; private set; }
    public int Score { get; private set; }

    public PlayerScoreEntryData(int kill, int death, int assist, int score)
    {
        Kill = kill;
        Death = death;
        Assist = assist;
        Score = score;
    }
    public void SetKill(int kill) => Kill = kill;
    public void SetDeath(int death) => Death = death;
    public void SetAssist(int assist) => Assist = assist;
    public void SetScore(int score) => Score = score;
}