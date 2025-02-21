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

    public UI_Minimap          Minimap        { get; private set; }
    public UI_Timer            Timer          { get; private set; }
    public UI_GameStatus       GameStatus     { get; private set; }
    public UI_SkillIndicator   SkillIndicator { get; private set; }

    void Awake()
    {
        SingletonInitialize();

        // UI 할당
        Minimap        = transform.GetChild(0).GetComponent<UI_Minimap>();
        Timer          = transform.GetChild(1).GetComponent<UI_Timer>();
        GameStatus     = transform.GetChild(2).GetComponent<UI_GameStatus>();
        SkillIndicator = transform.GetChild(3).GetComponent<UI_SkillIndicator>();
    }
}