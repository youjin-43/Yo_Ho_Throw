using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UI_Minimap_Helper))]
[RequireComponent(typeof(PhotonView))]
public class UI_Minimap : UI_Base
{
    #region VARIABLES
    private Transform _playerTransform;
    private Transform _minimapFrame;
    private Camera    _minimapCamera;

    Dictionary<int, MinimapIndicator> playerIndicatorDict = new Dictionary<int, MinimapIndicator>();

    // 원래 부모, 인디케이터
    DoubleTransform _playerIndicator;
    List<MapIndicatorInfo> _otherIndicator = new List<MapIndicatorInfo>();

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
    public void RemovePlayerTransform(int actorNumber)
    {
        MapIndicatorInfo target = new MapIndicatorInfo(null, null);

        foreach (MapIndicatorInfo info in _otherIndicator)
        {
            if (info.indicator.owner == actorNumber)
            {
                target = info;

                break;
            }
        }
        _otherIndicator.Remove(target);

        playerIndicatorDict.Remove(actorNumber);
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
        //{
        //    _playerTransform = InGameUIManager.Instance.PlayerTransform;
        //}

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
        MapIndicatorInfo otherIndicator = new MapIndicatorInfo(minimapIndicator.transform, minimapIndicator);

        if (isPlayer == true)
        {
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _playerIndicator.Item1 = minimapIndicator.transform;
                _playerIndicator.Item2 = minimapIndicator.indicator.transform;

                minimapIndicator.MyPlayerSetting();
            }
            else
            {
                minimapIndicator.OtherPlayerSetting();
            }

            minimapIndicator.owner = actorNumber;
            playerIndicatorDict[actorNumber] = minimapIndicator;
        }
        else
        {
            minimapIndicator.TreasureBoxSetting();
        }
        _otherIndicator.Add(otherIndicator);
    }
    [PunRPC]
    private void ShowPlayerIcon(int myActorNr, int targetActorNr, MinimapIconType iconType)
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
    }
    [PunRPC]
    private void HidePlayerIcon(int myActorNr, int targetActorNr, MinimapIconType iconType)
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
    }
    [PunRPC]
    public void SetPlayerIconRPC(int targetActorNr, MinimapIconType iconType)
    {
        int myActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

        if (myActorNr == targetActorNr) return;

        if (!playerIndicatorDict.ContainsKey(targetActorNr) || playerIndicatorDict[targetActorNr] == null) return;

        switch (iconType)
        {
            case MinimapIconType.Other_Player:
                playerIndicatorDict[targetActorNr].OtherPlayerSetting(); break;

            case MinimapIconType.Bounty_Hunter:
                playerIndicatorDict[targetActorNr].BountyHunterSetting(); break;
        }
    }
    [PunRPC]
    public void SetRevengeTargetIconRPC(int myActorNr, int targetActorNr)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != myActorNr) return;

        playerIndicatorDict[targetActorNr].RevengeTargetSetting();
    }
    public void SetPlayerIcon(int targetActorNr, MinimapIconType iconType)
    {
        photonView.RPC("SetPlayerIconRPC", RpcTarget.All, targetActorNr, iconType);
    }
    public void SetRevengeTargetIcon(int myActorNr, int targetActorNr)
    {
        photonView.RPC("SetRevengeTargetIconRPC", RpcTarget.All, myActorNr, targetActorNr);
    }
    private void AdjustIndicator()
    {
        if(_playerIndicator.Item1 == null)
        {
            return;
        }

        foreach(var info in _otherIndicator)
        {
            // 물체를 바라보는 Look
            Vector3 look = (info.transform.position - _playerIndicator.Item1.position).normalized;

            // 먼 거리에 있는 물체를 미니맵 가장자리에 그리게 하는 부분
            if (Vector3.Distance(_playerIndicator.Item1.position, info.transform.position) >= 9.9f)
            {
                // Adjust
                info.indicator.indicator.transform.position = _playerIndicator.Item2.position + (look * 9.9f);
            }
            else
            {
                info.indicator.indicator.transform.position = new Vector3(info.transform.position.x, 50f, info.transform.position.z);
            }

            // 플레이어의 Look
            Vector3 playerLook = _playerTransform.transform.forward.normalized;

            if (info.indicator.isAlwaysShow)
            {
                if (!info.indicator.gameObject.activeSelf)
                {
                    info.indicator.gameObject.SetActive(true);
                }
            }
            else
            {
                // 플레이어 시야각에만 그려지도록 하는 부분
                if (Vector3.Dot(look, playerLook) >= 0.7f)
                {
                    if (!info.indicator.gameObject.activeSelf)
                    {
                        info.indicator.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (info.indicator.gameObject.activeSelf)
                    {
                        info.indicator.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    #endregion
}
public struct MapIndicatorInfo
{
    public Transform transform;
    public MinimapIndicator indicator;

    public MapIndicatorInfo(Transform transform, MinimapIndicator indicator)
    {
        this.transform = transform;
        this.indicator = indicator;
    }
}
public struct DoubleTransform
{
    public Transform Item1;
    public Transform Item2;
}
public enum MinimapIconType
{
    My_Player,
    Other_Player,
    Bounty_Hunter,
    Revenge_Target,
    Treasure_Box,
}