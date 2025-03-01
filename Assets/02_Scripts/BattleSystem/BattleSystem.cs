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

    [SerializeField] int timeLimit = 300;

    int spawnedPlayerCount = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // TODO 찬규 : UI부분에서 추후 리셋을 구현하셨을 경우 호출
        //InGameUIManager.Instance.ResetUI();

        Debug.Log("현재 방 마스터 클라이언트 ID : " + PhotonNetwork.CurrentRoom.masterClientId.ToString());
        Debug.Log("현재 클라이언트 ID : " + PhotonNetwork.LocalPlayer.ActorNumber.ToString());

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
        // 플레이어 스폰
        yield return PlayerSpawnManager.Instance.SpawnCoroutine();

        // 모든 플레이어가 스폰되어 준비가 될 때까지 대기
        while (PhotonNetwork.CurrentRoom.PlayerCount != spawnedPlayerCount)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 화면을 활성화 함
        ScreenTransition.FadeOutRPC();

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.BattleStart,
            Time.unscaledTime,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
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
        KillLogPanelController.Instance.AddKillLog(killerActorNumber, victimActorNumber);

        // 마스터 클라이언트가 아닐 때는 반환
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("크아아아");

        // 리스폰 시킴 ( 물론 내부적으로 필요할 때만 리스폰 시킴 )
        PlayerSpawnManager.Instance.ExecuteRPC(RaiseEventCode.RespawnPlayer.ToString(), victimActorNumber);
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
        BattleUIController.Instance.EndGame();

        ScoreManager.Instance.EndGame();

        PlayerSpawnManager.Instance.EndGame();
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
        Debug.Log("로그 위치 확인 !");

        StartCoroutine(BattleCoroutine());
    }
    public static void SpawnCheck()
    {
        Instance.photonView.RPC("SpawnCheckRPC", RpcTarget.All);
    }
    [PunRPC]
    public void SpawnCheckRPC()
    {
        spawnedPlayerCount++;
    }
    virtual protected void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    virtual protected void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}