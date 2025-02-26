using UnityEngine;

public class UI_UITestScript : MonoBehaviour
{
    private void Start()
    {
        if (InGameUIManager.Instance.Minimap != null)
        {
            Debug.Log("미니맵 할당 완료");
        }
        if (InGameUIManager.Instance.Timer != null)
        {
            Debug.Log("타이머 할당 완료");
        }
        if (InGameUIManager.Instance.ScoreHUD != null)
        {
            Debug.Log("스코어 HUD 할당 완료");
        }
        if (InGameUIManager.Instance.ScorePanel != null)
        {
            Debug.Log("스코어 패널 할당 완료");
        }
        if (InGameUIManager.Instance.SkillIndicator != null)
        {
            Debug.Log("스킬창 할당 완료");
        }
        if (InGameUIManager.Instance.HealthIndicator != null)
        {
            Debug.Log("체력창 할당 완료");
        }
        if (InGameUIManager.Instance.Menu != null)
        {
            Debug.Log("메뉴창 할당 완료");
        }
        if (InGameUIManager.Instance.Setting != null)
        {
            Debug.Log("설정창 할당 완료");
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
            InGameUIManager.Instance.ResetUI();
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
        if (Input.GetKey(KeyCode.Tab))
        {
            InGameUIManager.Instance.ShowScorePanelUI(true);
        }
        else
        {
            InGameUIManager.Instance.ShowScorePanelUI(false);
        }
    }
}
