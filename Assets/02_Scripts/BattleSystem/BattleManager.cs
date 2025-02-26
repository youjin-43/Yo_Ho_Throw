using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour, IOnEventCallback
{
    static BattleManager instance = null;

    [SerializeField] int timeLimit = 300;

    Dictionary<int, int> revengeTargetDict = new Dictionary<int, int>();

    const int REVENGE_BONUS_REWARD = 1;

    int comboKill = 0;

    int spawnedPlayerCount = 0;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        // TODO 찬규 : UI부분에서 추후 리셋을 구현하셨을 경우 호출
        //InGameUIManager.Instance.ResetUI();

        BattleSetting();
    }
    public void BattleSetting()
    {
        revengeTargetDict.Clear();

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // ID 별 초기 값 -1을 할당한다
            revengeTargetDict[actorNumber] = -1;
        }

        StartCoroutine(BattleSettingCoroutine());
    }
    IEnumerator BattleSettingCoroutine()
    {
        Debug.Log("BattleSettingCoroutine Test AAA");
        // 플레이어 스폰
        yield return PlayerSpawnManager.Instance.SpawnCoroutine();

        Debug.Log("BattleSettingCoroutine Test BBB");
        // 모든 플레이어가 스폰되어 준비가 될 때까지 대기
        while (PhotonNetwork.CountOfPlayersInRooms != spawnedPlayerCount)
        {
            Debug.Log("PhotonNetwork.CountOfPlayers : " + PhotonNetwork.CountOfPlayers.ToString());
            Debug.Log("PhotonNetwork.CountOfPlayersOnMaster : " + PhotonNetwork.CountOfPlayersOnMaster.ToString());
            Debug.Log("PhotonNetwork.CountOfPlayersInRooms : " + PhotonNetwork.CountOfPlayersInRooms.ToString());

            Debug.Log("spawnedPlayerCount : " + spawnedPlayerCount.ToString());

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("BattleSettingCoroutine Test CCC");

        // 화면을 활성화 함
        ScreenTransition.FadeOutRPC();

        Debug.Log("BattleSettingCoroutine Test DDD");

        // 1초 대기
        yield return new WaitForSeconds(1f);

        Debug.Log("BattleSettingCoroutine Test EEE");

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.BattleStart,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendUnreliable);
    }

    /// <summary>
    /// 플레이어 간의 킬이 발생했을 때 호출하는 함수
    /// 자살이 일어났을 경우에는 죽은 사람의 ActorNumber를 killerActorNumber 및 victimActorNumber로 전달하면 된다
    /// </summary>
    /// <param name="killerActorNumber"> 죽인 사람 </param>
    /// <param name="victimActorNumber"> 죽은 사람 </param>
    public static void RegisterKill(int killerActorNumber, int victimActorNumber)
    {
        KillLogPanelController.AddKillLog(
            PhotonNetwork.CurrentRoom.Players[killerActorNumber].NickName,
            PhotonNetwork.CurrentRoom.Players[victimActorNumber].NickName);

        if (killerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            instance.comboKill++;

            BattleUIController.Instance.SetComboKill(instance.comboKill);
        }
        else if (victimActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            instance.comboKill = 0;

            BattleUIController.Instance.SetComboKill(instance.comboKill);
        }

        // 호스트가 아닌 경우 반환시킴
        if (!PhotonNetwork.IsMasterClient) return;

        // 복수 대상일 때
        if (instance.revengeTargetDict[killerActorNumber] == victimActorNumber)
        {
            instance.revengeTargetDict[killerActorNumber] = -1;

            InGameUIManager.HidePlayerIcon(victimActorNumber);

            ScoreManager.Instance.AddScore(killerActorNumber, victimActorNumber, REVENGE_BONUS_REWARD);
        }

        // 복수 대상이 아닐 때
        else
        {
            ScoreManager.Instance.AddScore(killerActorNumber, victimActorNumber);
        }

        // 죽은 플레이어 리스폰 요청
        PlayerSpawnManager.Instance.ExecuteRPC(
                    RaiseEventCode.RespawnPlayer.ToString(), victimActorNumber);

        // 타살일 경우 죽인 자는 죽은 자의 복수 대상으로 갱신
        instance.revengeTargetDict[victimActorNumber] = victimActorNumber != killerActorNumber ? killerActorNumber : instance.revengeTargetDict[victimActorNumber];
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.BattleStart:
                BattleStart(); break;
        }
    }
    void BattleStart()
    {
        StartCoroutine(BattleCoroutine());
    }
    IEnumerator BattleCoroutine()
    {
        Debug.Log("BattleCoroutine Test AAA");

        int seconds = timeLimit;

        int startDelay = 2;

        WaitForSeconds wait = new WaitForSeconds(1f);

        BattleUIController.Instance.SetLimitTimeText(seconds);


        while (startDelay > 0)
        {
            BattleUIController.Instance.SetBattleStartText(startDelay--);
            Debug.Log("BattleCoroutine Test BBB");

            yield return wait;
        }
        Debug.Log("BattleCoroutine Test CCC");
        BattleUIController.Instance.SetBattleStartText(startDelay);


        Debug.Log("BattleCoroutine Test DDD");
        while (seconds > 0)
        {
            yield return wait;

            seconds -= 1;

            CheckTime(seconds);

            BattleUIController.Instance.SetLimitTimeText(seconds);
            Debug.Log("BattleCoroutine Test EEE");
        }
        EndGameByTimeout();
    }
    void CheckTime(int seconds)
    {
        if (seconds == 60) // 제한시간이 1분 밖에 안 남았을 때
        {
            ScoreManager.Instance.SetIsFinalMinute(true);
        }
        
        if (seconds % 60 == 0 && seconds != 0) // TODO 찬규 : 1분마다 현상금 이벤트 발생
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ScoreManager.Instance.SetBountyTarget();
            }
        }
    }
    void EndGameByTimeout()
    {
        BattleUIController.Instance.EndGame();

        ScoreManager.Instance.EndGame();

        PlayerSpawnManager.Instance.EndGame();

        //if (!PhotonNetwork.IsMasterClient) return;

        //PhotonNetwork.RaiseEvent(
        //    (byte)RaiseEventCode.DeactivatePlayer,
        //    null,
        //    new RaiseEventOptions { Receivers = ReceiverGroup.All },
        //    SendOptions.SendUnreliable);
    }
    public static void SpawnCheck()
    {
        Debug.Log("before spawnedPlayerCount : " + instance.spawnedPlayerCount.ToString());

        instance.spawnedPlayerCount++;

        Debug.Log("after spawnedPlayerCount : " + instance.spawnedPlayerCount.ToString());
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);

    //public override void OnLeftRoom() // 로컬 플레이어(자신)가 방을 떠날 때 호출
    //{
    //    base.OnLeftRoom();
    //}
    //public override void OnPlayerLeftRoom(Player otherPlayer) // 타 플레이어가 방을 떠날 때 호출
    //{
    //    base.OnPlayerLeftRoom(otherPlayer);
    //}
}
