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
        if (InGameUIManager.Instance.StatusIndicator == null)
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

        // 게임이 시작되면 상점UI를 제외한 모든 UI를 Off 함
        InGameUIManager.Instance.GameStart();
    }


    private bool isGameStart = false;

    void Update()
    {
        // 상점UI가 켜져 있다는 뜻은 게임이 아직 시작되지 않았다는 뜻
        if(InGameUIManager.Instance.IsStoreUIOpen() == true)
        {

        }
        else
        {
            UIControl();
        }
    }

    public void UIControl()
    {
        if (Input.GetMouseButtonDown(0))
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F 아이템 사용");
            InGameUIManager.Instance.UseSkill(3, 5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InGameUIManager.Instance.ResetAllUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InGameUIManager.Instance.AddDagger();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InGameUIManager.Instance.AddDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InGameUIManager.Instance.AddHealth(1);
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
            // InGameUIManager.Instance.Death(5f); // StartCoroutine으로 하지 않을 경우 실행안됨

            StartCoroutine(InGameUIManager.Instance.Death(5f));
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
