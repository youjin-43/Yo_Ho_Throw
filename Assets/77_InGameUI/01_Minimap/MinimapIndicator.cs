using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIndicator : MonoBehaviour
{
    // 미니맵에 그려져야 할 오브젝트들에게 부착

    [SerializeField] private bool   IsPlayer = false;
    [SerializeField] public  Sprite Icon     = null;
    [SerializeField] public  Color  Color    = new Color(0, 1, 0, 1);

    public GameObject indicator;

    void Start()
    {
        indicator = new GameObject("MinimapIndicator");

        indicator.transform.SetParent(transform);
        indicator.transform.localPosition = Vector3.zero;

        indicator.AddComponent<SpriteRenderer>();

        if(Icon != null)
        {
            indicator.GetComponent<SpriteRenderer>().sprite = Icon;
        }

        indicator.GetComponent<SpriteRenderer>().color = Color;

        indicator.transform.position = new Vector3(indicator.transform.position.x, 50, indicator.transform.position.z);
        indicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        indicator.gameObject.layer = LayerMask.NameToLayer("Minimap");

        // 기존
        // ValueTuple<Transform, Transform> pair = new ValueTuple<Transform, Transform>(transform, indicator.transform);
        // InGameUIManager.Instance.BindIndicator(pair, IsPlayer);

        int actorNumber = 0;

        if (IsPlayer == true)
        {
            actorNumber = GetComponent<PhotonView>().OwnerActorNr;
        }

        // 통째로 넘김
        InGameUIManager.Instance.BindIndicator(actorNumber, this, IsPlayer);
    }
}