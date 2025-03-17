using Photon.Pun;
using UnityEngine;

public class ItemHandler : MonoBehaviourPun
{
    public static ItemHandler Instance { get; private set; } = null;

    Potion selectedPotion = null;
    public PlayerController playerController = null;

    private void Awake()
    {
        if (photonView.IsMine) 
        {
            Instance = this;

            playerController = GetComponent<PlayerController>();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Use();
    }
    public void Equip(Potion potion)
    {
        selectedPotion = potion;
    }
    public void UnEquip()
    {
        selectedPotion = null;

        InGameUIManager.Instance.SkillIndicator.HideItemSlotImage();
    }
    public void Use()
    {
        // 포션이 없다면 종료
        if (selectedPotion == null) return;

        // 살아있지 않은 상태에서 키 입력 시 종료
        if (!playerController.isAlive) return;

        // 포션 사용
        selectedPotion.Use();

        // 장착 해제
        UnEquip();
    }
}
