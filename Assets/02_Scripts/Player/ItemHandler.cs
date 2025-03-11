using Photon.Pun;
using UnityEngine;

public class ItemHandler : MonoBehaviourPun
{
    public static ItemHandler Instance { get; private set; } = null;

    Potion selectedPotion = null;

    private void Awake()
    {
        if (photonView.IsMine) Instance = this;
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
        if (selectedPotion == null) return;

        selectedPotion.Use();

        UnEquip();
    }
}
