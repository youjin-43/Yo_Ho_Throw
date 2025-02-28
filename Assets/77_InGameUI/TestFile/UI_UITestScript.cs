using UnityEngine;

public class UI_UITestScript : MonoBehaviour
{
    private void Start()
    {
        if (InGameUIManager.Instance.Minimap == null)
        {
            Debug.LogError("미니맵 할당 안됨");
        }
        if (InGameUIManager.Instance.Timer == null)
        {
            Debug.LogError("타이머 할당 안됨");
        }
        if (InGameUIManager.Instance.RealtimeScoreboard == null)
        {
            Debug.LogError("스코어 HUD 할당 안됨");
        }
        if (InGameUIManager.Instance.ScoreBoard == null)
        {
            Debug.LogError("스코어 패널 할당 안됨");
        }
        if (InGameUIManager.Instance.SkillIndicator == null)
        {
            Debug.LogError("스킬창 할당 안됨");
        }
        if (InGameUIManager.Instance.HealthIndicator == null)
        {
            Debug.LogError("체력창 할당 안됨");
        }
        if (InGameUIManager.Instance.Menu == null)
        {
            Debug.LogError("메뉴창 할당 안됨");
        }
        if (InGameUIManager.Instance.Setting == null)
        {
            Debug.LogError("설정창 할당 안됨");
        }
        if (InGameUIManager.Instance.KillLog == null)
        {
            Debug.LogError("킬 팝업창 할당 안됨");
        }
        if (InGameUIManager.Instance.DeathPopup == null)
        {
            Debug.LogError("사망 팝업창 할당 안됨");
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("좌클릭 스킬 발동");
            InGameUIManager.Instance.UseSkill(0, 2);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("우클릭 스킬 발동");
            InGameUIManager.Instance.UseSkill(1, 1f);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Shift 스킬 발동");
            InGameUIManager.Instance.UseSkill(2, 5);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            InGameUIManager.Instance.ResetAllUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InGameUIManager.Instance.AddDagger();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InGameUIManager.Instance.OnHealthChangedDebug(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InGameUIManager.Instance.OnHealthChangedDebug(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            InGameUIManager.Instance.StartTimer(180f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            InGameUIManager.Instance.ToggleMenuUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            InGameUIManager.Instance.AddKillLog("죽인사람", "죽은사람");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            InGameUIManager.Instance.Death(5f);
        }
        if (Input.GetKey(KeyCode.Tab))
        {
            InGameUIManager.Instance.ShowScoreboardUI(true);
        }
        else
        {
            InGameUIManager.Instance.ShowScoreboardUI(false);
        }
    }
}
