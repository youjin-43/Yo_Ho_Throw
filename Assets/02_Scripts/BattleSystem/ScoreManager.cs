using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static ScoreManager Instance { get; private set; } = null;

    Dictionary<int, PlayerScoreEntryData> playerScoreEntryDict = new Dictionary<int, PlayerScoreEntryData>();

    const int KILL_SCORE_REWARD = 1;

    int bountyTargetActorNumber = -1;

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
            playerScoreEntryDict[actorNumber] = new PlayerScoreEntryData(0, 0, 0, actorNumber);
        }
        isFinalMinute = false;
        isGameRunning = true;
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

        // 자살이 아닌 경우
        if (killerActorNumber != victimActorNumber)
        {
            if (victimActorNumber == bountyTargetActorNumber)
            {
                bonusReward += 2;

                int targetActorNr = bountyTargetActorNumber;

                foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
                {
                    InGameUIManager.HidePlayerIcon(kvp.Key, targetActorNr, MinimapIconType.Other_Player);
                    Debug.Log("숨기기 타겟 액터넘버 : " + targetActorNr.ToString());
                }

                PlayerSpawnManager.Instance.ExecuteRPC(RaiseEventCode.ScheduleBountyTargetDeactivation.ToString(), bountyTargetActorNumber);

                photonView.RPC("SetBountyTargetActorNumber", RpcTarget.Others, -1);
                bountyTargetActorNumber = -1;
            }

            Debug.Log("획득 전 점수 : " + playerScoreEntryDict[killerActorNumber].Score.ToString());
            Debug.Log("얻어야 하는 점수 : " + (KILL_SCORE_REWARD + bonusReward).ToString());

            playerScoreEntryDict[killerActorNumber].SetScore(
            playerScoreEntryDict[killerActorNumber].Score +
            (KILL_SCORE_REWARD + bonusReward) *
            (isFinalMinute ? 2 : 1)
            );
            Debug.Log("현재 점수 : " + playerScoreEntryDict[killerActorNumber].Score.ToString());

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

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.SaveData,
            PlayerScoreEntryData.ToHashtable(playerScoreEntryDict),
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
    int GetTopScorerActorNumber()
    {
        var sortedScores = new List<KeyValuePair<int, PlayerScoreEntryData>>(playerScoreEntryDict);

        sortedScores.Sort((x, y) => y.Value.Score.CompareTo(x.Value.Score));

        return sortedScores[0].Key;
    }
    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.SaveData:
                SaveData(photonEvent); break;
        }
    }
    public void SetBountyTarget()
    {
        bountyTargetActorNumber = GetTopScorerActorNumber();

        Debug.Log("바운티 타겟 설정 : " + bountyTargetActorNumber.ToString());

        photonView.RPC("SetBountyTargetActorNumber", RpcTarget.All, bountyTargetActorNumber);
    }
    [PunRPC]
    public void SetBountyTargetActorNumber(int targetActorNr)
    {
        Debug.Log("바운티 타겟 ActorNr 확인 : " + targetActorNr.ToString());
        if (targetActorNr != -1)
        {
            InGameUIManager.ShowPlayerIcon(targetActorNr, MinimapIconType.Bounty_Hunter);

            if (bountyTargetActorNumber != -1)
            {
                PlayerSpawnManager.Instance.ExecuteRPC(RaiseEventCode.DeactivateBountyTargetImmediate.ToString(), bountyTargetActorNumber);
            }

            PlayerSpawnManager.Instance.ExecuteRPC(RaiseEventCode.ActivateBountyTarget.ToString(), targetActorNr);
        }
        bountyTargetActorNumber = targetActorNr;
    }
    void SaveData(EventData photonEvent)
    {
        Hashtable eventContent = (Hashtable)photonEvent.CustomData;

        if (playerScoreEntryDict == null)
        {
            // 기존 Dictionary가 null이면 새로 생성
            playerScoreEntryDict = PlayerScoreEntryData.FromHashtable(eventContent);
        }
        else
        {
            // 기존 Dictionary를 재사용하여 업데이트
            playerScoreEntryDict.Clear(); // 기존 데이터 제거

            foreach (DictionaryEntry entry in eventContent)
            {
                int key = (int)entry.Key;
                Hashtable data = (Hashtable)entry.Value;

                // 기존 객체를 재사용하기 위해 값만 변경
                if (playerScoreEntryDict.TryGetValue(key, out PlayerScoreEntryData existingData))
                {
                    existingData.SetKill((int)data["kill"]);
                    existingData.SetDeath((int)data["death"]);
                    existingData.SetScore((int)data["score"]);
                }
                else
                {
                    // 새로운 데이터라면 새 객체 추가
                    playerScoreEntryDict[key] = new PlayerScoreEntryData(
                        (int)data["kill"],
                        (int)data["death"],
                        (int)data["score"],
                        (int)data["actorNumber"]
                    );
                }
            }
        }
    }
}

[Serializable]
public class PlayerScoreEntryData
{
    int kill;
    int death;
    int score;
    int actorNumber;
    public int Kill { get => kill; }
    public int Death { get => death; }
    public int Score { get => score; }
    public int ActorNumber { get => actorNumber; }
    public PlayerScoreEntryData(int kill, int death, int score, int actorNumber)
    {
        this.kill = kill;
        this.death = death;
        this.score = score;
        this.actorNumber = actorNumber;
    }
    public void SetKill(int kill) => this.kill = kill;
    public void SetDeath(int death) => this.death = death;
    public void SetScore(int score) => this.score = score;
    public static Hashtable ToHashtable(Dictionary<int, PlayerScoreEntryData> dict)
    {
        Hashtable hashtable = new Hashtable();

        foreach (var kvp in dict)
        {
            Hashtable entryData = new Hashtable {
            { "kill", kvp.Value.Kill },
            { "death", kvp.Value.Death },
            { "score", kvp.Value.Score },
            { "actorNumber", kvp.Value.ActorNumber }
        };

            hashtable[kvp.Key] = entryData; // key(int) - value(Hashtable) 형태로 저장
        }

        return hashtable;
    }
    public static Dictionary<int, PlayerScoreEntryData> FromHashtable(Hashtable hashtable)
    {
        Dictionary<int, PlayerScoreEntryData> dict = new Dictionary<int, PlayerScoreEntryData>();

        foreach (DictionaryEntry entry in hashtable)
        {
            int key = (int)entry.Key; // Key를 int로 변환
            Hashtable data = (Hashtable)entry.Value; // Value를 Hashtable로 변환

            PlayerScoreEntryData playerData = new PlayerScoreEntryData(
                (int)data["kill"],
                (int)data["death"],
                (int)data["score"],
                (int)data["actorNumber"]
            );

            dict[key] = playerData;
        }

        return dict;
    }
}
