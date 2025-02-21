using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } = null;

    Dictionary<int, int> playerScores = new Dictionary<int, int>();

    int killScoreReward = 1;

    private void Awake()
    {
        Instance = this;

        Init();
    }
    public void Init()
    {
        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // ActorNumber와 매칭해서 점수를 0으로 초기화한다
            playerScores[actorNumber] = 0;
        }

        killScoreReward = 1;
    }
    public void AddScore(int killerActorNumber, int victimActorNumber)
    {
        // 점수 추가
        //playerScores[killerActorNumber] += GetKillScoreReward(victimActorNumber);
    }
    public void ModifyKillReward(int reward)
    {
        killScoreReward = reward;
    }
    //int GetKillScoreReward(int victimActorNumber)
    //{

    //}
}
