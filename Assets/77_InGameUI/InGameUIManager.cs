using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    #region SINGLETON
    private static InGameUIManager instance;
    public  static InGameUIManager Instance
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
            DontDestroyOnLoad(gameObject);
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

    [Header("ScorePanel")]
    [SerializeField] public PlayerScoreEntry playerScoreEntryPrefab;
    #endregion

    public UI_Minimap         Minimap         { get; private set; }
    public UI_Timer           Timer           { get; private set; }
    public UI_ScoreHUD        ScoreHUD        { get; private set; }
    public UI_ScorePanel      ScorePanel      { get; private set; }
    public UI_SkillIndicator  SkillIndicator  { get; private set; }
    public UI_HealthIndicator HealthIndicator { get; private set; }
    public UI_Menu            Menu            { get; private set; }
    public UI_Setting         Setting         { get; private set; }

    void Awake()
    {
        SingletonInitialize();

        // UI 할당
        Minimap         = transform.GetChild(0).GetComponent<UI_Minimap>();
        Timer           = transform.GetChild(1).GetComponent<UI_Timer>();
        ScoreHUD        = transform.GetChild(2).GetComponent<UI_ScoreHUD>();
        ScorePanel      = transform.GetChild(3).GetComponent<UI_ScorePanel>();
        SkillIndicator  = transform.GetChild(4).GetComponent<UI_SkillIndicator>();
        HealthIndicator = transform.GetChild(5).GetComponent<UI_HealthIndicator>();
        Menu            = transform.GetChild(6).GetComponent<UI_Menu>();
        Setting         = transform.GetChild(7).GetComponent<UI_Setting>();
    }





    #region COMMON
    /// <summary>
    /// 라운드가 종료되거나, 다시 시작될 때 호출해 주세요
    /// </summary>
    public void ResetUI()
    {
        Minimap        .ResetUI();
        Timer          .ResetUI();
        ScoreHUD       .ResetUI();
        SkillIndicator .ResetUI();
        HealthIndicator.ResetUI();
        Menu           .ResetUI();
        Setting        .ResetUI();
    }

    /// <summary>
    /// 팝업 UI가 활성화 되어있는지 판단하는 함수입니다.
    /// </summary>
    /// <returns></returns>
    public bool IsPopupUIOpen()
    {
        if(Menu.gameObject.activeSelf == true || Setting.gameObject.activeSelf == true)
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

    public static void ShowPlayerIcon(int targetActorNumber)
    {
        instance.Minimap.ShowPlayerIcon(targetActorNumber);
    }
    public static void HidePlayerIcon(int targetActorNumber)
    {
        instance.Minimap.HidePlayerIcon(targetActorNumber);
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





    #region SCORE HUD
    /// <summary>
    /// 점수 HUD 온오프 함수입니다
    /// </summary>
    public void ToggleScoreHUD()
    {
        ScoreHUD.ToggleScorePanelUI();
    }

    /// <summary>
    /// 점수 HUD를 초기화 하는곳에서 호출해 주세요
    /// </summary>
    public void InitScoreHUD(int order, string nickName)
    {
        ScoreHUD.InitScoreHUD(order, nickName);
    }

    /// <summary>
    /// 점수 HUD 데이터를 갱신하는 곳에서 호출해 주세요
    /// </summary>
    /// <param name="order"></param>
    /// <param name="nickName"></param>
    /// <param name="rank"></param>
    /// <param name="score"></param>
    public void UpdateScoreHUDData(int order, string nickName, int rank, int score)
    {
        ScoreHUD.UpdateScoreHUDData(order, nickName, rank, score);
    }
    #endregion





    #region SCORE PANEL
    /// <summary>
    /// 점수 패널 UI 온오프 함수입니다
    /// </summary>
    public void ShowScorePanelUI(bool isVisible)
    {
        ScorePanel.ShowScorePanelUI(isVisible);
    }

    /// <summary>
    /// 점수 패널을 초기화 하는곳에서 호출해 주세요
    /// </summary>
    /// <param name="actorNumber"></param>
    /// <param name="nickName"></param>
    public void InitScorePanel(int actorNumber, string nickName)
    {
        ScorePanel.InitScorePanel(playerScoreEntryPrefab, actorNumber, nickName);
    }

    public void UpdateScorePanelData()
    {

    }

    #endregion





    #region SKILL INDICATOR
    /// <summary>
    /// 플레이어가 스킬을 사용할 때 호출해 주세요
    /// </summary>
    /// <param name="button">입력한 버튼(0 : 좌클릭, 1 : 우클릭, 2 : Shift)</param>
    /// <param name="time">해당 스킬 쿨타임</param>
    public void UseSkill(int button, float time)
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





    #region HEALTH INDICATOR
    /// <summary>
    /// 체력이 변경되었을 때 호출해 주세요
    /// </summary>
    /// <param name="currentHealth">변경 후 현재 체력</param>
    public void OnHealthChanged(int currentHealth)
    {
        HealthIndicator.OnHealthChanged(currentHealth);
    }

    public void OnHealthChangedDebug(int healthDelta)
    {
        HealthIndicator.OnHealthChangedDebug(healthDelta);
    }
    #endregion





    #region MENU
    /// <summary>
    /// 메뉴 UI 온오프 함수입니다
    /// </summary>
    public void ToggleMenuUI()
    {
        if(Setting.gameObject.activeSelf == false)
        {
            Menu.ToggleMenuUI();
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
        if(Setting.gameObject.activeSelf == true)
        {
            Menu.ToggleMenuUI();
        }

        Setting.ToggleSettingUI();

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
}