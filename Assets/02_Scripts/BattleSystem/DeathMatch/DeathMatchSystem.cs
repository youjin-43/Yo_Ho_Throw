using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class DeathMatchSystem : BattleSystem
{
    Dictionary<int, int> revengeTargetDict = new Dictionary<int, int>();

    const int REVENGE_BONUS_REWARD = 1;

    int comboKill = 0;

    public override void BattleSetting()
    {
        revengeTargetDict.Clear();

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // ID 별 초기 값 -1을 할당한다
            revengeTargetDict[actorNumber] = -1;
        }

        base.BattleSetting();
    }

    /// <summary>
    /// 플레이어 간의 킬이 발생했을 때 호출하는 함수
    /// 자살이 일어났을 경우에는 죽은 사람의 ActorNumber를 killerActorNumber 및 victimActorNumber로 전달하면 된다
    /// </summary>
    /// <param name="killerActorNumber"> 죽인 사람 </param>
    /// <param name="victimActorNumber"> 죽은 사람 </param>
    public override void RegisterKill(int killerActorNumber, int victimActorNumber)
    {
        base.RegisterKill(killerActorNumber, victimActorNumber);

        if (killerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            comboKill++;

            BattleUIController.Instance.SetComboKill(comboKill);
        }
        else if (victimActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            comboKill = 0;

            BattleUIController.Instance.SetComboKill(comboKill);
        }

        // 호스트가 아닌 경우 반환시킴
        if (!PhotonNetwork.IsMasterClient) return;

        // 복수 대상일 때
        if (revengeTargetDict[killerActorNumber] == victimActorNumber)
        {
            revengeTargetDict[killerActorNumber] = -1;

            InGameUIManager.HidePlayerIcon(victimActorNumber);

            ScoreManager.Instance.AddScore(killerActorNumber, victimActorNumber, REVENGE_BONUS_REWARD);
        }

        // 복수 대상이 아닐 때
        else
        {
            ScoreManager.Instance.AddScore(killerActorNumber, victimActorNumber);
        }

        InGameUIManager.HidePlayerIcon(victimActorNumber);

        // 타살일 경우 죽인 자는 죽은 자의 복수 대상으로 갱신
        revengeTargetDict[victimActorNumber] = victimActorNumber != killerActorNumber ? killerActorNumber : revengeTargetDict[victimActorNumber];
    }
    protected override void CheckTime(int seconds)
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
    protected override IEnumerator BattleCoroutine()
    {
        int startDelay = 2;

        WaitForSeconds wait = new WaitForSeconds(1f);

        BattleUIController.Instance.SetBattleStartText(startDelay--);

        yield return new WaitForSeconds(1f);

        BattleUIController.Instance.SetBattleStartText(startDelay--);

        yield return new WaitForSeconds(1f);

        BattleUIController.Instance.SetBattleStartText(startDelay);

        StartLimitedTimer();
    }
}
