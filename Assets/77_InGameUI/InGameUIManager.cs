using UnityEngine;

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

    public UI_Minimap         Minimap         { get; private set; }
    public UI_Timer           Timer           { get; private set; }
    public UI_GameStatus      GameStatus      { get; private set; }
    public UI_SkillIndicator  SkillIndicator  { get; private set; }
    public UI_HealthIndicator HealthIndicator { get; private set; }

    void Awake()
    {
        SingletonInitialize();

        // UI 할당
        Minimap         = transform.GetChild(0).GetComponent<UI_Minimap>();
        Timer           = transform.GetChild(1).GetComponent<UI_Timer>();
        GameStatus      = transform.GetChild(2).GetComponent<UI_GameStatus>();
        SkillIndicator  = transform.GetChild(3).GetComponent<UI_SkillIndicator>();
        HealthIndicator = transform.GetChild(4).GetComponent<UI_HealthIndicator>();
    }





    #region COMMON
    /// <summary>
    /// 라운드가 종료되거나, 다시 시작될 때 호출해 주세요
    /// </summary>
    public void ResetUI()
    {
        Minimap        .ResetUI();
        Timer          .ResetUI();
        GameStatus     .ResetUI();
        SkillIndicator .ResetUI();
        HealthIndicator.ResetUI();
    }
    #endregion





    #region MINIMAP

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





    #region GAME STATUS

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
}