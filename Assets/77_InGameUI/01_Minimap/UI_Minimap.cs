using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] Transform PlayerTransform;
    [SerializeField] Camera    MinimapCamera;

    private Transform _minimapFrame;

    // 원래 부모, 인디케이터
    ValueTuple<Transform, Transform> _playerIndicator;
    List<ValueTuple<Transform, Transform>> _otherIndicator = new List<(Transform, Transform)>();

    private Transform _playerAngle;

    void Awake()
    {
        _minimapFrame = transform.GetChild(0);
        _playerAngle  = MinimapCamera.transform.GetChild(1);
    }

    void Update()
    {
        AdjustIndicator();
    }

    void LateUpdate()
    {
        MinimapCamera.transform.position = new Vector3(PlayerTransform.position.x, 100f, PlayerTransform.position.z);
        //MinimapCamera.transform.rotation = Quaternion.Euler(90f, PlayerTransform.localEulerAngles.y, 0f);
        _minimapFrame.rotation = Quaternion.Euler(0f, 0f, -PlayerTransform.localEulerAngles.y);
        _playerAngle.localRotation = Quaternion.Euler(0f, 0f, -PlayerTransform.localEulerAngles.y);
    }

    public void ResetUI()
    {

    }

    public void BindIndicator(ValueTuple<Transform, Transform> pair, bool isPlayer)
    {
        if(isPlayer == true)
        {
            _playerIndicator = pair;
        }
        else
        {
            _otherIndicator.Add(pair);
        }
    }

    private void AdjustIndicator()
    {
        if(_playerIndicator.Item1 == null)
        {
            return;
        }

        foreach(var indicator in _otherIndicator)
        {
            if(Vector3.Distance(_playerIndicator.Item1.position, indicator.Item1.position) >= 9.9f)
            {
                // Look
                Vector3 look = (indicator.Item1.position - _playerIndicator.Item1.position).normalized;

                // Adjust
                indicator.Item2.position = _playerIndicator.Item2.position + (look * 9.9f);
            }
            else
            {
                indicator.Item2.position = new Vector3(indicator.Item1.position.x, 50f, indicator.Item1.position.z);
            }
        }
    }
}
