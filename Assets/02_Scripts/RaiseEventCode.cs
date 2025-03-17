using UnityEngine;

public enum RaiseEventCode
{
    #region 플레이어 코인 관리 이벤트
    EditClientCoin = 20,
    EditHostCoin = 21,

    #region BattleSystem 이벤트

    BattleStart = 30,
    SaveData = 31,

    #endregion

    #region Player 이벤트

    SpawnPlayer = 40,
    RespawnPlayer = 41,
    ActivatePlayer = 42,
    DeactivatePlayer = 43,
    ActivateBountyTarget = 44,
    DeactivateBountyTargetImmediate = 45,
    ScheduleBountyTargetDeactivation = 46,
    #endregion

    #region UI 갱신 이벤트 [ 각 플레이어의 Kill, Death, Assist, Score 텍스트 변경 ]

    UpdateKillCount = 50,
    UpdateDeathCount = 51,
    UpdateScore = 52,
    UpdateRank = 53,
    UpdateRealtime = 54,
    ResetScoreboard = 55,

    #endregion
}
