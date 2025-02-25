using System;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIndicator : MonoBehaviour
{
    // 미니맵에 그려져야 할 오브젝트들에게 부착

    [SerializeField] private bool   IsPlayer = false;
    [SerializeField] private Sprite Icon     = null;
    [SerializeField] private Color  Color    = new Color(0, 1, 0, 1);

    void Start()
    {
        GameObject _indicator = new GameObject("MinimapIndicator");

        _indicator.transform.SetParent(transform);
        _indicator.transform.localPosition = Vector3.zero;


        _indicator.AddComponent<SpriteRenderer>();

        if(Icon != null)
        {
            _indicator.GetComponent<SpriteRenderer>().sprite = Icon;
        }

        _indicator.GetComponent<SpriteRenderer>().color = Color;

        _indicator.transform.position = new Vector3(_indicator.transform.position.x, 50, _indicator.transform.position.z);
        _indicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        _indicator.gameObject.layer = LayerMask.NameToLayer("Minimap");

        ValueTuple<Transform, Transform> pair = new ValueTuple<Transform, Transform>(transform, _indicator.transform);

        InGameUIManager.Instance.BindIndicator(pair, IsPlayer);
    }
}