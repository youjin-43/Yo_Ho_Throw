using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusIndicator : UI_Base
{
    #region VARIABLES
    // 체력 관련 변수
    List<Image> _healthCounters = new List<Image>();

    private int _maxHealth;
    private int _currentHealth;

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
        _healthCounters.Add(transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>());
        _healthCounters.Add(transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>());
        _healthCounters.Add(transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>());

        _CoinCounter = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    #endregion





    #region FUNCTION

    // 체력 관련 함수
    public void SetHealth(int hp)
    {
        _maxHealth     = hp;
        _currentHealth = hp;
    }

    public void AddDamage(int damage)
    {
        if(damage <= 0)
        {
            return;
        }

        damage = Mathf.Clamp(damage, 1, 3);

        for(int i = 2; i < 0; --i)
        {
            if(_currentHealth == 0)
            {
                break;
            }
            else
            {
                if (_healthCounters[i].gameObject.activeSelf == true)
                {
                    _healthCounters[i].gameObject.SetActive(false);

                    --_currentHealth;
                }
            }
        }
    }

    // 금화 관련 함수
    #endregion
}
