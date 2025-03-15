using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

//public enum GameMode
//{
//    DeathMatch,
//    ZombieMode
//}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (!_instance)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    _instance = obj.AddComponent(typeof(GameManager)) as GameManager;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public string UserName { get; set; } = "soldier";

    private void Start()
    {
        CursorController.Instance.CursorEnable();
    }




    // 마우스 민감도 설정값 저장
    private GameObject _player;
    private float      _sensitivity = 4.5f;

    public float GetStoreSensitivityValue()
    {
        return _sensitivity;
    }
    public void StoreSensitivityValue(float value)
    {
        SetSensitivity(_sensitivity = value);
    }

    public void CachePlayer(GameObject player)
    {
        if(player.GetComponent<PhotonView>().IsMine == true)
        {
            _player = player;

            SetSensitivity(_sensitivity);
        }
    }

    private void SetSensitivity(float value)
    {
        if(_player != null)
        {
            _player.GetComponent<PlayerController>().SetMouseSensitivity(_sensitivity);
        }
    }

    // 소리 설정값 저장
    private bool _masterVolumeOn = true;
    private bool _EffectVolumeOn = true;

    public void StoreIsMasterVolumeOn(bool isOn)
    {
        _masterVolumeOn = isOn;
    }
    public bool IsMasterVolumeOn()
    {
        return _masterVolumeOn;
    }

    public void StoreIsEffectVolumeOn(bool isOn)
    {
        _EffectVolumeOn = isOn;
    }
    public bool IsEffectVolumeOn()
    {
        return _EffectVolumeOn;
    }
}