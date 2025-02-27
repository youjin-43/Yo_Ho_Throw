using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    private Transform _playerTransform;
    private Transform _minimapFrame;
    private Camera    _minimapCamera;

    Dictionary<int, MinimapIndicator> playerIndicatorDict = new Dictionary<int, MinimapIndicator>();

    // 원래 부모, 인디케이터
    ValueTuple<Transform, Transform> _playerIndicator;
    List<ValueTuple<Transform, Transform>> _otherIndicator = new List<(Transform, Transform)>();

    private Transform _playerAngle;

    void Awake()
    {
        _minimapFrame = transform.GetChild(0);
    }

    void Start()
    {
        _playerTransform = InGameUIManager.Instance.PlayerTransform;
        _minimapCamera   = InGameUIManager.Instance.MinimapCamera;

        _playerAngle = _minimapCamera.transform.GetChild(1);
    }

    void Update()
    {
        AdjustIndicator();
    }

    void LateUpdate()
    {
        _minimapCamera.transform.position = new Vector3(_playerTransform.position.x, 100f, _playerTransform.position.z);
        //MinimapCamera.transform.rotation = Quaternion.Euler(90f, PlayerTransform.localEulerAngles.y, 0f);
        _minimapFrame.rotation     = Quaternion.Euler(0f, 0f, -_playerTransform.localEulerAngles.y);
        _playerAngle.localRotation = Quaternion.Euler(0f, 0f, -_playerTransform.localEulerAngles.y);
    }

    public void ResetUI()
    {

    }

    public void BindIndicator(int actorNumber, MinimapIndicator minimapIndicator, bool isPlayer)
    {
        if(isPlayer == true)
        {
            _playerIndicator.Item1 = minimapIndicator.transform;
            _playerIndicator.Item2 = minimapIndicator.indicator.transform;

            playerIndicatorDict[actorNumber] = minimapIndicator;
        }
        else
        {
            ValueTuple<Transform, Transform> otherIndicator;
            {
                otherIndicator.Item1 = minimapIndicator.transform;
                otherIndicator.Item2 = minimapIndicator.indicator.transform;
            }
            _otherIndicator.Add(otherIndicator);
        }
    }

    public void ShowPlayerIcon(int targetActorNumber)
    {
        // ActorNumber를 통해 Icon 오브젝트 활성화
        playerIndicatorDict[targetActorNumber].gameObject.SetActive(true);
    }
    public void HidePlayerIcon(int targetActorNumber)
    {
        // ActorNumber를 통해 Icon 오브젝트 비활성화
        playerIndicatorDict[targetActorNumber].gameObject.SetActive(false);
    }

    private void AdjustIndicator()
    {
        if(_playerIndicator.Item1 == null)
        {
            return;
        }

        foreach(var indicator in _otherIndicator)
        {
            // 물체를 바라보는 Look
            Vector3 look = (indicator.Item1.position - _playerIndicator.Item1.position).normalized;

            // 먼 거리에 있는 물체를 미니맵 가장자리에 그리게 하는 부분
            if (Vector3.Distance(_playerIndicator.Item1.position, indicator.Item1.position) >= 9.9f)
            {
                // Adjust
                indicator.Item2.position = _playerIndicator.Item2.position + (look * 9.9f);
            }
            else
            {
                indicator.Item2.position = new Vector3(indicator.Item1.position.x, 50f, indicator.Item1.position.z);
            }

            // 플레이어의 Look
            Vector3 playerLook = _playerTransform.transform.forward.normalized;

            // 플레이어 시야각에만 그려지도록 하는 부분
            if (Vector3.Dot(look, playerLook) >= 0.7f)
            {
                indicator.Item2.gameObject.SetActive(true);
            }
            else
            {
                indicator.Item2.gameObject.SetActive(false);
            }

            // 근데 전부 그려져도 되나?
            // 벽 뒤에 숨은건?

            Ray ray = new Ray(_playerTransform.position, playerLook);

            if(Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Water")))
            {
                Debug.Log("Test");
            }
        }
    }

}
