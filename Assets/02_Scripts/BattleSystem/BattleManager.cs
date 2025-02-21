using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {

    }
    /// <summary>
    /// 플레이어 간의 킬이 발생했을 때 호출하는 함수
    /// </summary>
    /// <param name="killerActorNumber">죽인 사람</param>
    /// <param name="victimActorNumber">죽은 사람</param>
    public static void RegisterKill(int killerActorNumber, int victimActorNumber)
    {
        // 호스트가 아닌 경우 반환시킴
        if (!PhotonNetwork.IsMasterClient) return;

        // 처치에 성공한 플레이어에게 점수를 추가
        ScoreManager.Instance.AddScore(killerActorNumber, victimActorNumber);
    }

    //public override void OnLeftRoom() // 로컬 플레이어(자신)가 방을 떠날 때 호출
    //{
    //    base.OnLeftRoom();
    //}
    //public override void OnPlayerLeftRoom(Player otherPlayer) // 타 플레이어가 방을 떠날 때 호출
    //{
    //    base.OnPlayerLeftRoom(otherPlayer);
    //}
}