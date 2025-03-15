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
    private float _sensitivity = 0;

    public void StoreSensitivityValue(float value)
    {
        _sensitivity = value;
    }
    public float GetStoreSensitivityValue()
    {
        return _sensitivity;
    }

    private GameObject _player;

    public void CachePlayer(GameObject player)
    {
        if(player.GetComponent<PhotonView>().IsMine == true)
        {
            _player = player;

            if(_sensitivity != 0)
            {
                player.GetComponent<PlayerController>().SetMouseSensitivity(_sensitivity);
            }
        }
    }
}