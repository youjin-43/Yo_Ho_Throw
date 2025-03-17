using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class UI_StatusIndicator : UI_Base
{
    #region VARIABLES
    // ID 관련 변수
    private TextMeshProUGUI _playerID;

    // 체력 관련 변수
    private List<Image> _healthCounters = new List<Image>();

    private int _maxHealth     = 3;
    private int _currentHealth = 3;

    // 금화 관련 변수
    private TextMeshProUGUI _CoinCounter;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
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
        foreach(var item in _healthCounters)
        {
            item.gameObject.SetActive(true);
        }

        _currentHealth = _maxHealth;
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _playerID = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        _healthCounters.Add(transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>());
        _healthCounters.Add(transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>());
        _healthCounters.Add(transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>());

        _CoinCounter = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        _CoinCounter.text = "0";
    }
    #endregion





    #region FUNCTION
    #region ID
    public void SetPlayerID(string id)
    {
        _playerID.text = id;
    }
    #endregion

    #region HEALTH
    public void SetHealth(int hp)
    {
        _maxHealth     = hp;
        _currentHealth = hp;
    }

    public void AddDamage(int damage)
    {
        if(damage <= 0 || _currentHealth == 0)
        {
            return;
        }

        damage = Mathf.Clamp(damage, 1, 3);

        int addDamagePoint = 0;

        for(int i = 2; i >= 0; --i)
        {
            if(addDamagePoint == damage)
            {
                break;
            }

            if (_healthCounters[i].gameObject.activeSelf == true)
            {
                _healthCounters[i].gameObject.SetActive(false);

                --_currentHealth;
                ++addDamagePoint;
            }
            else
            {
                continue;
            }
        }
    }

    public void AddHealth(int health)
    {
        if(health <= 0 || _currentHealth == 3)
        {
            return;
        }

        int addHealthPoint = 0;

        for(int i = 0; i < 3; ++i)
        {
            if(addHealthPoint == health)
            {
                break;
            }

            if (_healthCounters[i].gameObject.activeSelf == false)
            {
                _healthCounters[i].gameObject.SetActive(true);

                ++_currentHealth;
                ++addHealthPoint;
            }
            else
            {
                continue;
            }
        }
    }
    #endregion

    #region GOLDCOIN
    public void SetGoldCoin(int coin, int actorNumber)
    {
        GetComponent<PhotonView>().RPC("SetGoldCoinRPC", PhotonNetwork.CurrentRoom.Players[actorNumber], coin);
    }
    [PunRPC]
    public void SetGoldCoinRPC(int coin)
    {
        Debug.Log("-----------SetGoldCoinRPC-----------");

        _CoinCounter.text = coin.ToString();
    }
    #endregion
    #endregion
}
