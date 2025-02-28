using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_SkillIndicator : UI_Base
{
    #region VARIABLES
    // 스킬 쿨타임 이펙트
    private Image _skill_Shift_CooldownEffect;
    private Image _skill_LClick_CooldownEffect;
    private Image _skill_RClick_CooldownEffect;

    // 스킬 비활성화 이펙트(우클릭(단검 던지기) 전용)
    private Image _skillActivation;

    // 단검 갯수 카운터
    private List<GameObject> _daggerCounter = new List<GameObject>();

    private int _numOfDagger = 5;
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
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _name = name;
        _isAlwaysVisible = true;

        // 쿨타임 이펙트
        _skill_Shift_CooldownEffect  = transform.GetChild(4).transform.GetChild(1).GetComponent<Image>();
        _skill_LClick_CooldownEffect = transform.GetChild(5).transform.GetChild(1).GetComponent<Image>();
        _skill_RClick_CooldownEffect = transform.GetChild(6).transform.GetChild(1).GetComponent<Image>();

        _skill_Shift_CooldownEffect.gameObject.SetActive(false);
        _skill_LClick_CooldownEffect.gameObject.SetActive(false);
        _skill_RClick_CooldownEffect.gameObject.SetActive(false);

        // 스킬 비활성화 이펙트(우클릭(단검 던지기) 전용)
        _skillActivation = transform.GetChild(6).GetChild(2).GetComponent<Image>();
        _skillActivation.gameObject.SetActive(false);

        // 단검 갯수 카운트
        Transform daggerCounter = transform.GetChild(6).GetChild(4).transform;

        for(int i = 0; i < 5; ++i)
        {
            _daggerCounter.Add(daggerCounter.GetChild(i).GetChild(0).gameObject);
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
            StartCoroutine(CooldownEffect(_skill_Shift_CooldownEffect, cooldownTime));
        }
        // 좌클릭
        else if (button == 0)
        {
            StartCoroutine(CooldownEffect(_skill_LClick_CooldownEffect, cooldownTime));
        }
        // 우클릭
        else if (button == 1)
        {
            if(_numOfDagger != 0)
            {
                RemoveDagger();
                StartCoroutine(CooldownEffect(_skill_RClick_CooldownEffect, cooldownTime));
            }
        }
    }

    private IEnumerator CooldownEffect(Image coolDownImage, float cooldownTime)
    {
        coolDownImage.gameObject.SetActive(true);
        coolDownImage.fillAmount = 1f;

        float elapsedTime = 0f;

        while(elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            coolDownImage.fillAmount = 1f - (elapsedTime / cooldownTime);
            yield return null;
        }

        coolDownImage.gameObject.SetActive(false);
    }
    #endregion

    #region DAGGERCOUNT
    public void AddDagger(int count)
    {
        count = Mathf.Clamp(count, 1, 5);

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

    public void RemoveDagger()
    {
        for(int i = 4; i >= 0; --i)
        {
            if(_daggerCounter[i].activeSelf == true)
            {
                // 마지막 단검까지 카운터를 Off한다는 뜻은
                // 전부 다 던졌다는 소리
                // 그렇다면 우클릭은 비활성화 이펙트 ON
                if(i == 0)
                {
                    _skillActivation.gameObject.SetActive(true);
                }

                _numOfDagger = Mathf.Clamp(_numOfDagger -= 1, 0, 5);

                _daggerCounter[i].SetActive(false);
                return;
            }
        }
    }
    #endregion
    #endregion
}
