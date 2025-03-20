using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

//public enum GameMode
//{
//    DeathMatch,
//    ZombieMode
//}

public enum PlayerColor
{
    Blue,
    Red,
    Yellow
}

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

    public PlayerColor selectedSkinColor = PlayerColor.Blue; // ??? ???? ?? ?? (???: ?? )

    /// <summary>
    /// ??? ??? ???? ??
    /// </summary>
    public void SelectColor(PlayerColor color)
    {
        selectedSkinColor = color;
        Debug.Log($"??? ??: {selectedSkinColor}");
    }

    public bool isPlayerStop;

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

    // ???? ???? ????
    private bool _isBgmOn = true;
    private bool _isSfxOn = true;
    private float _bgmValue = 0.2f;
    private float _sfxValue = 0.8f;

    public void StoreBgmCheckState(bool state)
    {
        _isBgmOn = state;
    }
    public void StoreSfxCheckState(bool state)
    {
        _isSfxOn = state;
    }
    public bool GetBgmCheckState()
    {
        return _isBgmOn;
    }
    public bool GetSfxCheckState()
    {
        return _isSfxOn;
    }
    public void StoreBgmValue(float value)
    {
        _bgmValue = value; 
    }
    public float GetBgmValue()
    {
        return _bgmValue;
    }
    public void StoreSfxValue(float value)
    {
        _sfxValue = value;
    }
    public float GetSfxValue()
    {
        return _sfxValue;
    }

    // ???? ????
    private float _sensitivity = 10f;

    public void StoreSensitivity(float sensitivity)
    {
        _sensitivity = sensitivity;

        SetSensitivity();
    }
    public float GetSensitivity()
    {
        return _sensitivity;
    }
    public void SetSensitivity()
    {
        if (_player != null)
        {
            _player.GetComponent<PlayerController>().SetMouseSensitivity(_sensitivity);
        }
    }

    // ???????? ????
    private GameObject _player;

    public void StorePlayer(GameObject player)
    {
        if(player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            _player = player;

            SetSensitivity();
        }
    }

    public void PlayerStop(bool _isPlayerStop)
    {
        isPlayerStop = _isPlayerStop;
    }
}