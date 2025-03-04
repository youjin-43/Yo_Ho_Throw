using Photon.Pun;
using System;
using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    // 미니맵에 그려져야 할 오브젝트들에게 부착

    [SerializeField] private bool   IsPlayer = false;

    public GameObject indicator {  get; private set; }

    SpriteRenderer spriteRenderer = null;

    void Start()
    {
        indicator = new GameObject("MinimapIndicator");

        indicator.transform.SetParent(transform);
        indicator.transform.localPosition = Vector3.zero;

        spriteRenderer = indicator.AddComponent<SpriteRenderer>();

        indicator.transform.position = new Vector3(indicator.transform.position.x, 50, indicator.transform.position.z);
        indicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        indicator.gameObject.layer = LayerMask.NameToLayer("Minimap");

        // 기존
        // ValueTuple<Transform, Transform> pair = new ValueTuple<Transform, Transform>(transform, indicator.transform);
        // InGameUIManager.Instance.BindIndicator(pair, IsPlayer);

        int actorNumber = 0;

        if (IsPlayer == true)
        {
            actorNumber = GetComponentInParent<PhotonView>().OwnerActorNr;
        }

        // 통째로 넘김
        if (InGameUIManager.Instance != null) InGameUIManager.Instance.BindIndicator(actorNumber, this, IsPlayer);
    }
    public void MyPlayerSetting()
    {
        spriteRenderer.sprite = InGameUIManager.Instance.minimapIconColor.my_Player_Sprite;
        spriteRenderer.color = InGameUIManager.Instance.minimapIconColor.my_Player_Color;
    }
    public void OtherPlayerSetting()
    {
        spriteRenderer.sprite = InGameUIManager.Instance.minimapIconColor.other_Player_Sprite;
        spriteRenderer.color = InGameUIManager.Instance.minimapIconColor.other_Player_Color;
    }
    public void BountyHunterSetting()
    {
        spriteRenderer.sprite = InGameUIManager.Instance.minimapIconColor.bounty_Hunter_Sprite;
        spriteRenderer.color = InGameUIManager.Instance.minimapIconColor.bounty_Hunter_Color;
    }
    public void RevengeTargetSetting()
    {
        spriteRenderer.sprite = InGameUIManager.Instance.minimapIconColor.revenge_Target_Sprite;
        spriteRenderer.color = InGameUIManager.Instance.minimapIconColor.revenge_Target_Color;
    }
    public void TreasureBoxSetting()
    {
        spriteRenderer.sprite = InGameUIManager.Instance.minimapIconColor.treasure_Box_Sprite;
        spriteRenderer.color = InGameUIManager.Instance.minimapIconColor.treasure_Box_Color;
    }
}