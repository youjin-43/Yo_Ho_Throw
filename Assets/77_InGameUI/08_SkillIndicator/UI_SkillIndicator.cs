using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_SkillIndicator : UI_Base
{
    #region VARIABLES
    // 스킬 활성화 이펙트
    private GameObject _skill_Shift_ActiveEffect;
    private GameObject _skill_LClick_ActiveEffect;
    private GameObject _skill_RClick_ActiveEffect;
    private GameObject _skill_Item_ActiveEffect;

    // 스킬 쿨타임 이펙트
    private Image _skill_Shift_CooldownEffect;
    private Image _skill_LClick_CooldownEffect;
    private Image _skill_RClick_CooldownEffect;

    // 스킬 비활성화 이펙트(우클릭(단검 던지기), 아이템)
    private Image _skillActivation;
    private Image _itemActivation;

    // 단검 갯수 카운터
    private List<GameObject> _daggerCounter = new List<GameObject>();

    private int _numOfDagger = 5;

    // 아이템 슬롯 이미지
    private Image _itemSlot;
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
        StopAllCoroutines();

        AddDagger(5);

        _skill_Shift_CooldownEffect.fillAmount  = 1f;
        _skill_LClick_CooldownEffect.fillAmount = 1f;
        _skill_RClick_CooldownEffect.fillAmount = 1f;

        _skill_Shift_CooldownEffect .gameObject.SetActive(false);
        _skill_LClick_CooldownEffect.gameObject.SetActive(false);
        _skill_RClick_CooldownEffect.gameObject.SetActive(false);

        _skillActivation.gameObject.SetActive(false);
        _itemActivation.gameObject.SetActive(false);
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _name = name;
        _isAlwaysVisible = true;

        // 활성화 이펙트
        {
            _skill_Shift_ActiveEffect  = transform.GetChild(0).transform.GetChild(0).gameObject;
            _skill_LClick_ActiveEffect = transform.GetChild(1).transform.GetChild(0).gameObject;
            _skill_RClick_ActiveEffect = transform.GetChild(2).transform.GetChild(0).gameObject;
            _skill_Item_ActiveEffect   = transform.GetChild(3).transform.GetChild(0).gameObject;

            _skill_Item_ActiveEffect.gameObject.SetActive(false);
        }
        // 쿨타임 이펙트
        {
            _skill_Shift_CooldownEffect  = transform.GetChild(0).transform.GetChild(2).GetComponent<Image>();
            _skill_LClick_CooldownEffect = transform.GetChild(1).transform.GetChild(2).GetComponent<Image>();
            _skill_RClick_CooldownEffect = transform.GetChild(2).transform.GetChild(2).GetComponent<Image>();

            _skill_Shift_CooldownEffect.gameObject.SetActive(false);
            _skill_LClick_CooldownEffect.gameObject.SetActive(false);
            _skill_RClick_CooldownEffect.gameObject.SetActive(false);
        }
        // 스킬 비활성화 이펙트(우클릭(단검 던지기), 아이템)
        {
            _skillActivation = transform.GetChild(2).GetChild(3).GetComponent<Image>();
            _itemActivation  = transform.GetChild(3).GetChild(3).GetComponent<Image>();

            _skillActivation.gameObject.SetActive(false);
            _itemActivation.gameObject.SetActive(false);
        }
        // 단검 갯수 카운터
        {
            Transform daggerCounter = transform.GetChild(2).GetChild(5).transform;

            for (int i = 0; i < 5; ++i)
            {
                _daggerCounter.Add(daggerCounter.GetChild(i).GetChild(0).gameObject);
            }
        }
        // 아이템 슬롯
        {
            _itemSlot = transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Image>();
            _itemSlot.gameObject.SetActive(false);
        }
    }
    #endregion





    #region FUNCTION
    #region COOLDOWN
    public void StartCooldownEffect(int button, float cooldownTime)
    {
        // Shift
        if (button == 2)
        {
            StartCoroutine(CooldownEffect(_skill_Shift_ActiveEffect, _skill_Shift_CooldownEffect, cooldownTime));
        }
        // 좌클릭
        else if (button == 0)
        {
            StartCoroutine(CooldownEffect(_skill_LClick_ActiveEffect, _skill_LClick_CooldownEffect, cooldownTime));
        }
        // 우클릭
        else if (button == 1)
        {
            if (_numOfDagger != 0)
            {
                StartCoroutine(CooldownEffect(_skill_RClick_ActiveEffect, _skill_RClick_CooldownEffect, cooldownTime, true));
            }
        }
        // F(아이템 사용)
        else if(button == 3)
        {
            // 아이템은 쿨타임이 없음, 그냥 이펙트만 끄면 됨
            
            // 검은 배경은 키고
            _itemActivation.gameObject.SetActive(true);
            // 파티클은 끄고
            _skill_Item_ActiveEffect.SetActive(false);

            // 이미지도 없에면 좋을듯
            _itemSlot.gameObject.SetActive(false);
        }
    }

    private IEnumerator CooldownEffect(GameObject activeEffect, Image coolDownImage, float cooldownTime, bool isRClick = false)
    {
        activeEffect.SetActive(false);
        coolDownImage.gameObject.SetActive(true);
        coolDownImage.fillAmount = 1f;

        float elapsedTime = 0f;
        
        while(elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            coolDownImage.fillAmount = 1f - (elapsedTime / cooldownTime);
            yield return null;
        }

        if(isRClick == true)
        {
            if(_numOfDagger != 0)
            {
                activeEffect.SetActive(true);
            }
        }
        else
        {
            activeEffect.SetActive(true);
        }

        coolDownImage.gameObject.SetActive(false);
    }
    #endregion

    #region DAGGERCOUNT
    public void AddDagger(int count)
    {
        count = Mathf.Clamp(count, 1, 5);

        _skill_RClick_ActiveEffect.SetActive(true);
        _skillActivation.gameObject.SetActive(false);

        int addCounter = 0;

        foreach(GameObject counter in _daggerCounter)
        {
            if(addCounter == count)
            {
                break;
            }

            if(counter.activeSelf == false)
            {
                _numOfDagger = Mathf.Clamp(_numOfDagger += 1, 0, 5);

                counter.SetActive(true);
                ++addCounter;
            }
        }
    }

    public void RemoveDagger(int numOfDagger)
    {
        _numOfDagger = numOfDagger;

        Debug.Log("셋팅 시도 :" + numOfDagger.ToString());

        int i = 0;

        for (; i < _numOfDagger; i++) _daggerCounter[i].SetActive(true);

        for (; i < 5; i++) _daggerCounter[i].SetActive(false);

        if (_numOfDagger == 0)
        {
            _skill_RClick_ActiveEffect.SetActive(false);
            _skillActivation.gameObject.SetActive(true);
        }
    }
    #endregion

    #region ITEM
    public void SetItemSlotImage(Image image)
    {
        _itemSlot.gameObject.SetActive(true);
        _skill_Item_ActiveEffect.gameObject.SetActive(true);
        _itemSlot.sprite = image.sprite;
    }
    public void HideItemSlotImage()
    {
        _itemSlot.gameObject.SetActive(false);
        _skill_Item_ActiveEffect.gameObject.SetActive(false);
        _itemActivation.gameObject.SetActive(true);
    }
    #endregion
    #endregion
}
