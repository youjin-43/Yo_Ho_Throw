using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public abstract class BattleSystem : MonoBehaviourPun, IOnEventCallback
{
    public static BattleSystem Instance { get; private set; } = null;

    int timeLimit = 300;

    int spawnedPlayerCount = 0;

    int itemSelectedPlayerCount = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // 플레이어의 현재 씬 정보 저장 
        //PhotonManager.Instance.UpdatePlayerSceneProperty();

        // TODO 찬규 : UI부분에서 추후 리셋을 구현하셨을 경우 호출
        //InGameUIManager.Instance.ResetUI();

        // 호스트가 아니라면 반환
        if (!PhotonNetwork.IsMasterClient) return;

        BattleSetting();
    }
    protected abstract IEnumerator BattleCoroutine();
    public virtual void BattleSetting()
    {
        StartCoroutine(BattleSettingCoroutine());
    }
    protected virtual IEnumerator BattleSettingCoroutine()
    {
        //Debug.Log("배틀 셋팅 코루틴 함수");

        // 플레이어 스폰
        yield return PlayerSpawnManager.Instance.SpawnCoroutine();


        // 모든 플레이어가 스폰되어 준비가 될 때까지 대기
        while (PhotonNetwork.CurrentRoom.PlayerCount != spawnedPlayerCount)
        {
            //Debug.Log("스폰 대기, 현재 스폰 수 : " + spawnedPlayerCount.ToString() + ", 대기 중 플레이어 수 : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
            yield return new WaitForSeconds(0.5f);
        }

        //Debug.Log("플레이어 스폰 완료");

        // 화면 활성화 함
        ScreenTransition.FadeIn();

        BattleUIController.Instance.photonView.RPC("SetLimitTimeText", RpcTarget.All, timeLimit);

        yield return new WaitForSeconds(0.5f);

        // 아이템 패널 활성화
        InGameUIManager.Instance.ItemSelect.OnShowItemPanel();

        // 현재 방에 있는 사람의 수만큼 아이템 선택을 마쳤다면
        while (PhotonNetwork.CurrentRoom.PlayerCount != itemSelectedPlayerCount) yield return null;

        BattleUIController.Instance.HideWaitingScreen();

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.BattleStart,
            Time.unscaledTime,
            new RaiseEventOptions { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }
    /// <summary>
    /// 플레이어 간의 킬이 발생했을 때 호출하는 함수
    /// 자살이 일어났을 경우에는 죽은 사람의 ActorNumber를 killerActorNumber 및 victimActorNumber로 전달하면 된다
    /// </summary>
    /// <param name="killerActorNumber"> 죽인 사람 </param>
    /// <param name="victimActorNumber"> 죽은 사람 </param>
    public virtual void RegisterKill(int killerActorNumber, int victimActorNumber)
    {
        // 킬로그 발생
        //KillLogPanelController.Instance.AddKillLog(killerActorNumber, victimActorNumber);

        if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
        {
            PlayerSpawnManager.Instance.photonView.RPC("FullKnife", RpcTarget.All);
        }

        // 마스터 클라이언트가 아닐 때는 반환
        if (!PhotonNetwork.IsMasterClient) return;

        PlayerSpawnManager.Instance.photonView.RPC("KillSound", RpcTarget.All, killerActorNumber);

        // 리스폰 시킴 ( 물론 내부적으로 필요할 때만 리스폰 시킴 )
        PlayerSpawnManager.Instance.ExecuteRPC(RaiseEventCode.RespawnPlayer.ToString(), victimActorNumber);
    }
    [PunRPC]
    public void RegisterKillRPC(int killerActorNumber, int victimActorNumber)
    {
        RegisterKill(killerActorNumber, victimActorNumber);
    }
    protected virtual void CheckTime(int seconds)
    {
        return;
    }
    protected virtual void StartLimitedTimer()
    {
        StartCoroutine(LimitedTimerCoroutine());
    }
    protected virtual void EndGameByTimeout()
    {
        PlayerSpawnManager.Instance.currPlayerPhotonView.RPC("GameEndPlayer", RpcTarget.All);

        PlayerSpawnManager.Instance.DeactivatePlayer();

        BattleUIController.Instance.EndGame();

        ScoreManager.Instance.EndGame();

        PlayerSpawnManager.Instance.EndGame();

        ScreenTransition.Instance.FadeOutRPC();

        GameReadyUIManager.IsStartFade = false;

        if (PhotonNetwork.IsMasterClient)
            ScreenTransition.Instance.FadeActionSetting(() => PhotonManager.Instance.GoToReadyScene());
    }
    private IEnumerator LimitedTimerCoroutine()
    {
        int seconds = timeLimit;
        float startTime = Time.time + 1;

        while (seconds > 0)
        {
            yield return null;

            if (startTime <= Time.time)
            {
                int value = (int)((Time.time - startTime) % 1);

                startTime += value + 1;

                seconds -= value + 1;

                BattleUIController.Instance.SetLimitTimeText(seconds);

                CheckTime(seconds);
            }
        }

        EndGameByTimeout();
    }
    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.BattleStart: BattleStart(); break;
        }
    }
    private void BattleStart()
    {
        //Debug.Log("배틀 스타트 함수 호출");

        StartCoroutine(BattleCoroutine());
    }
    public static void SpawnCheck()
    {
        //Debug.Log("SpawnCheck Static 함수 호출");

        Instance.photonView.RPC("SpawnCheckRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void SpawnCheckRPC()
    {
        //Debug.Log("SpawnCheckRPC RPC 함수 호출");

        spawnedPlayerCount++;

        //Debug.Log("currentCount : " + spawnedPlayerCount);
    }
    public static void FirstItemSelect()
    {
        BattleUIController.Instance.ShowWaitingScreen();

        Instance.photonView.RPC("FirstItemSelectRPC", RpcTarget.All);
    }
    [PunRPC]
    public void FirstItemSelectRPC()
    {
        itemSelectedPlayerCount++;
    }
    virtual protected void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    virtual protected void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
}