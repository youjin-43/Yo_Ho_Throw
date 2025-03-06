using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    #region SINGLETON
    private static InGameUIManager instance;
    public static InGameUIManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            // 왜 접근하려 함? 돌아버린거냐
        }
    }

    void SingletonInitialize()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    #region VARIABLES
    /// <summary>
    /// 하위 UI들이 필요로 하는 변수는 매니저가 받아와서 하위 UI에 전달해줌
    /// </summary>
    [Header("Minimap")]
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public Camera    MinimapCamera;
    [SerializeField] public MinimapIconColor minimapIconColor;

    [Header("ScorePanel")]
    [SerializeField] public PlayerScoreEntry playerScoreEntryPrefab;

    [Header("KillLog")]
    [SerializeField] public KillLogPanel KillLogPanelPrefab;

    [Header("ItemSelect")]
    private bool _isItemSelected = false;
    #endregion

    public UI_DeathPopup         DeathPopup         { get; private set; }
    public UI_Minimap            Minimap            { get; private set; }
    public UI_Timer              Timer              { get; private set; }
    public UI_RealtimeScoreboard RealtimeScoreboard { get; private set; }
    public UI_Scoreboard         ScoreBoard         { get; private set; }
    public UI_SkillIndicator     SkillIndicator     { get; private set; }
    public UI_StatusIndicator    StatusIndicator    { get; private set; }
    public UI_Menu               Menu               { get; private set; }
    public UI_Setting            Setting            { get; private set; }
    public UI_KillLog            KillLog            { get; private set; }
    public UI_ItemSelect         ItemSelect         { get; private set; }

    private Dictionary<string, UI_Base> UIs = new Dictionary<string, UI_Base>();

    void Awake()
    {
        SingletonInitialize();

        // UI 할당
        UIs["DeathPopup"]         =  DeathPopup         = transform.GetChild( 0).GetComponent<UI_DeathPopup>();
        UIs["Minimap"]            =  Minimap            = transform.GetChild( 1).GetComponent<UI_Minimap>();
        UIs["Timer"]              =  Timer              = transform.GetChild( 2).GetComponent<UI_Timer>();
        UIs["RealtimeScoreboard"] =  RealtimeScoreboard = transform.GetChild( 3).GetComponent<UI_RealtimeScoreboard>();
        UIs["Scoreboard"]         =  ScoreBoard         = transform.GetChild( 4).GetComponent<UI_Scoreboard>();
        UIs["SkillIndicator"]     =  SkillIndicator     = transform.GetChild( 5).GetComponent<UI_SkillIndicator>();
        UIs["StatusIndicator"]    =  StatusIndicator    = transform.GetChild( 6).GetComponent<UI_StatusIndicator>();
        UIs["Menu"]               =  Menu               = transform.GetChild( 7).GetComponent<UI_Menu>();
        UIs["Setting"]            =  Setting            = transform.GetChild( 8).GetComponent<UI_Setting>();
        UIs["KillLog"]            =  KillLog            = transform.GetChild( 9).GetComponent<UI_KillLog>();
                                     ItemSelect         = transform.GetChild(10).GetComponent<UI_ItemSelect>();

        foreach (var ui in UIs)
        {
            ui.Value.Init();
        }
    }

    #region COMMON
    /// <summary>
    /// 게임이 시작될 때 호출해 주세요
    /// </summary>
    public void GameStart()
    {
        OffAllUI();

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
    }


    /// <summary>
    /// 라운드가 종료되거나, 다시 시작될 때 호출해 주세요
    /// </summary>
    public void ResetAllUI()
    {
        foreach (var ui in UIs)
        {
            ui.Value.ResetUI();
        }
    }

    /// <summary>
    /// 항상 켜져있어야 하는 UI를 On하는 함수입니다.
    /// </summary>
    public void OnAllUI()
    {
        foreach (var ui in UIs)
        {
            if(ui.Value._isAlwaysVisible == true)
            {
                ui.Value.On();
            }
        }
    }

    /// <summary>
    /// 모든 UI를 Off하는 함수입니다.
    /// </summary>
    public void OffAllUI(string exception = "")
    {
        foreach (var ui in UIs)
        {
            if(ui.Key != exception)
            {
                ui.Value.Off();
            }
        }
    }

    /// <summary>
    /// 팝업 UI가 활성화 되어있는지 판단하는 함수입니다.
    /// </summary>
    /// <returns></returns>
    public bool IsPopupUIOpen()
    {
        if (Menu.gameObject.activeSelf == true || Setting.gameObject.activeSelf == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsStoreUIOpen()
    {
        if(ItemSelect.gameObject.activeSelf == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion





    #region MINIMAP
    /// <summary>
    /// 미니맵 아이콘 조절용 함수
    /// </summary>
    /// <param name="indicatorDesc"></param>
    /// <param name="isPlayer"></param>
    public void BindIndicator(int actorNumber, MinimapIndicator minimapIndicatorDesc, bool isPlayer = false)
    {
        Minimap.BindIndicator(actorNumber, minimapIndicatorDesc, isPlayer);
    }

    public static void ShowPlayerIcon(int myActorNr, int targetActorNr, MinimapIconType iconType)
    {
        instance.Minimap.SetPlayerIcon(true, myActorNr, targetActorNr, iconType);
    }

    public static void HidePlayerIcon(int myActorNr, int targetActorNr, MinimapIconType iconType)
    {
        instance.Minimap.SetPlayerIcon(false, myActorNr, targetActorNr, iconType);
    }
    #endregion





    #region TIMER
    /// <summary>
    /// 라운드가 시작될 때 호출해 주세요
    /// </summary>
    /// <param name="time">라운드 제한시간 입니다.</param>
    public void StartTimer(float time)
    {
        Timer.StartTimer(time);
    }
    #endregion





    #region REALTIME SCOREBOARD
    /// <summary>
    /// 실시간 점수보드 UI 온오프 함수입니다
    /// </summary>
    public void ToggleRealtimeScoreboard()
    {
        RealtimeScoreboard.ToggleUI();
    }

    /// <summary>
    /// 실시간 점수보드 UI를 초기화 하는곳에서 호출해 주세요
    /// </summary>
    public RealtimePlayerScoreEntry InitRealtimeScoreboard(int order, string nickName)
    {
        return RealtimeScoreboard.InitRealtimeScoreboard(order, nickName);
    }

    /// <summary>
    /// 실시간 점수보드 UI 데이터를 갱신하는 곳에서 호출해 주세요
    /// </summary>
    /// <param name="order"></param>
    /// <param name="nickName"></param>
    /// <param name="rank"></param>
    /// <param name="score"></param>
    public void UpdateRealtimeScoreboardData(int order, string nickName, int rank, int score)
    {
        RealtimeScoreboard.UpdateRealtimeScoreboardData(order, nickName, rank, score);
    }
    #endregion





    #region SCOREBOARD
    /// <summary>
    /// 점수 보드 UI 온오프 함수입니다
    /// </summary>
    public void ShowScoreboardUI(bool isVisible)
    {
        ScoreBoard?.ShowScoreboardUI(isVisible);
    }

    /// <summary>
    /// 점수 보드를 초기화 하는곳에서 호출해 주세요
    /// </summary>
    /// <param name="actorNumber"></param>
    /// <param name="nickName"></param>
    public PlayerScoreEntry InitScoreboard(int actorNumber, string nickName)
    {
        return ScoreBoard.InitScoreboard(playerScoreEntryPrefab, actorNumber, nickName);
    }

    public void UpdateScoreboardData_DeathCount(int actorNumber, int deathCount)
    {
        ScoreBoard.UpdateScoreboardData_DeathCount(actorNumber, deathCount);
    }

    public void UpdateScoreboardData_Score(int actorNumber, int score)
    {
        ScoreBoard.UpdateScoreboardData_Score(actorNumber, score);
    }

    public void UpdateScoreboardData_KillCount(int actorNumber, int killCount)
    {
        ScoreBoard.UpdateScoreboardData_KillCount(actorNumber, killCount);
    }

    public void ResetScoreboard()
    {
        ScoreBoard.ResetScoreboard();
    }
    #endregion





    #region SKILL INDICATOR
    /// <summary>
    /// 플레이어가 스킬을 사용할 때 호출해 주세요
    /// </summary>
    /// <param name="button">입력한 버튼(0 : 좌클릭, 1 : 우클릭, 2 : Shift, 3 : F)</param>
    /// <param name="time">해당 스킬 쿨타임</param>
    public void UseSkill(int button, float time = 0f)
    {
        SkillIndicator.StartCooldownEffect(button, time);
    }

    /// <summary>
    /// 단검 갯수가 증가될 때 호출해 주세요
    /// </summary>
    public void AddDagger(int count = 1)
    {
        SkillIndicator.AddDagger(count);
    }
    #endregion





    #region STATUS INDICATOR
    /// <summary>
    /// 공격을 받았을 때 호출해 주세요
    /// </summary>
    /// <param name="currentHealth">입은 대미지</param>
    public void AddDamage(int damage)
    {
        StatusIndicator.AddDamage(damage);
    }
    #endregion





    #region MENU
    /// <summary>
    /// 메뉴 UI 온오프 함수입니다
    /// </summary>
    public void ToggleMenuUI()
    {
        if (Setting.gameObject.activeSelf == false)
        {
            Menu.ToggleUI();
        }

        Cursor.visible = IsPopupUIOpen();

        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion





    #region SETTING
    /// <summary>
    /// 설정 UI 온오프 함수입니다
    /// </summary>
    public void ToggleSettingUI()
    {
        if (Setting.gameObject.activeSelf == true)
        {
            Menu.ToggleUI();
        }

        Setting.ToggleUI();

        Cursor.visible = IsPopupUIOpen();

        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion





    #region KILLLOG
    /// <summary>
    /// 킬 판정하는 곳에서 호출해 주세요
    /// </summary>
    /// <param name="killerNickname"></param>
    /// <param name="victimNickname"></param>
    public void AddKillLog(string killerNickname, string victimNickname)
    {
        KillLog.AddKillLog(KillLogPanelPrefab, killerNickname, victimNickname);
    }

    public void ReturnPanel(KillLogPanel killLogPanel)
    {
        KillLog.ReturnPanel(killLogPanel);
    }
    #endregion





    #region DEATH POPUP
    /// <summary>
    /// 플레이어가 사망할 때 호출해 주세요
    /// </summary>
    /// <param name="respawnTime">리스폰 시간</param>
    public IEnumerator Death(float respawnTime)
    {
        yield return DeathPopup.DeathPopupActive(respawnTime);
    }
    #endregion





    #region ITEM SELECT
    /// <summary>
    /// UI_ItemSelect <-> UI_SkillIndicator
    /// </summary>
    /// <param name="image"></param>
    public void ItemSelected(Image image)
    {
        SkillIndicator.SetItemSlotImage(image);

        ItemSelect.gameObject.SetActive(false);

        OnAllUI();

        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 아이템을 선택했는지 확인하는 곳에서 호출해 주세요
    /// </summary>
    /// <returns></returns>
    public bool IsItemSelected()
    {
        return _isItemSelected;
    }
    #endregion
}