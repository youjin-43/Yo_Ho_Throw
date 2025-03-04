using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class UI_Minimap : UI_Base
{
    #region VARIABLES
    private Transform _playerTransform;
    private Transform _minimapFrame;
    private Camera    _minimapCamera;

    Dictionary<int, MinimapIndicator> playerIndicatorDict = new Dictionary<int, MinimapIndicator>();

    // 원래 부모, 인디케이터
    ValueTuple<Transform, Transform> _playerIndicator;
    List<ValueTuple<Transform, Transform>> _otherIndicator = new List<(Transform, Transform)>();

    private Transform _playerAngle;
    private PhotonView photonView = null;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;

        photonView = GetComponent<PhotonView>();
    }
    
    public override void On()
    {
        gameObject.SetActive(true);
    }

    public override void Off()
    {
        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {
    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _minimapFrame = transform.GetChild(0);
    }

    void Start()
    {
        // Map+InGameUI_TestScene Scene 디버그용, 나중에 주석처리하셈
        {
            _playerTransform = InGameUIManager.Instance.PlayerTransform;
        }

        _minimapCamera   = InGameUIManager.Instance.MinimapCamera;

        _playerAngle = _minimapCamera.transform.GetChild(1);
    }

    void Update()
    {
        AdjustIndicator();
    }

    void LateUpdate()
    {
        if (_playerTransform == null) return;
        _minimapCamera.transform.position = new Vector3(_playerTransform.position.x, 100f, _playerTransform.position.z);
        //MinimapCamera.transform.rotation = Quaternion.Euler(90f, PlayerTransform.localEulerAngles.y, 0f);
        _minimapFrame.rotation     = Quaternion.Euler(0f, 0f, -_playerTransform.localEulerAngles.y);
        _playerAngle.localRotation = Quaternion.Euler(0f, 0f, -_playerTransform.localEulerAngles.y);
    }
    #endregion





    #region FUNCTION
    public void BindIndicator(int actorNumber, MinimapIndicator minimapIndicator, bool isPlayer)
    {
        if (isPlayer == true)
        {
            _playerIndicator.Item1 = minimapIndicator.transform;
            _playerIndicator.Item2 = minimapIndicator.indicator.transform;

            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                minimapIndicator.MyPlayerSetting();
            else
                minimapIndicator.OtherPlayerSetting();

            playerIndicatorDict[actorNumber] = minimapIndicator;
        }
        else
        {
            ValueTuple<Transform, Transform> otherIndicator;
            {
                otherIndicator.Item1 = minimapIndicator.transform;
                otherIndicator.Item2 = minimapIndicator.indicator.transform;
            }
            minimapIndicator.TreasureBoxSetting();

            _otherIndicator.Add(otherIndicator);
        }
    }
    [PunRPC]
    private void ShowPlayerIcon(int targetActorNumber)
    {
        // ActorNumber를 통해 Icon 오브젝트 활성화
        playerIndicatorDict[targetActorNumber].indicator.gameObject.SetActive(true);
    }
    [PunRPC]
    private void HidePlayerIcon(int targetActorNr)
    {
        // ActorNumber를 통해 Icon 오브젝트 비활성화
        playerIndicatorDict[targetActorNr].indicator.gameObject.SetActive(false);

        Debug.Log(playerIndicatorDict[targetActorNr].gameObject.name + " 비활성화 아이콘 확인");
    }
    public void SetPlayerIcon(bool isShow, int myActorNr, int targetActorNr, MinimapIconType iconType)
    {
        if (myActorNr == targetActorNr) return;

        switch (iconType)
        {
            case MinimapIconType.Other_Player:
                playerIndicatorDict[targetActorNr].OtherPlayerSetting(); break;

            case MinimapIconType.Bounty_Hunter:
                playerIndicatorDict[targetActorNr].BountyHunterSetting(); break;

            case MinimapIconType.Revenge_Target:
                playerIndicatorDict[targetActorNr].RevengeTargetSetting(); break;
        }
        

        if (isShow)
            photonView.RPC("ShowPlayerIcon", PhotonNetwork.CurrentRoom.Players[myActorNr], targetActorNr);
        else
            photonView.RPC("HidePlayerIcon", PhotonNetwork.CurrentRoom.Players[myActorNr], targetActorNr);
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
    #endregion
}

public enum MinimapIconType
{
    My_Player,
    Other_Player,
    Bounty_Hunter,
    Revenge_Target,
    Treasure_Box,
}