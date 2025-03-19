using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; } = null;

    [SerializeField] EffectKeyValuePair[] effectList;

    Dictionary<EffectType, ParticleSystem> effectDictionary = new Dictionary<EffectType, ParticleSystem>();

    private void Awake()
    {
        Init();
    }
    public void Play(Vector3 pos, EffectType effectType)
    {
        if (effectDictionary.ContainsKey(effectType))
        {
            effectDictionary[effectType].transform.rotation = Quaternion.identity;

            effectDictionary[effectType].transform.position = pos;

            effectDictionary[effectType].GetComponent<ParticleSystem>().Play();
        }
        else
        {
            Debug.LogWarning("재생하려는 이펙트가 설정되어 있지 않음 : " + effectType.ToString());
        }
    }
    public void Play(Vector3 pos, Vector3 eulerAngle, EffectType effectType)
    {
        if (effectDictionary.ContainsKey(effectType))
        {
            effectDictionary[effectType].transform.rotation = Quaternion.Euler(eulerAngle);

            effectDictionary[effectType].transform.position = pos;

            effectDictionary[effectType].GetComponent<ParticleSystem>().Play();
        }
        else
        {
            Debug.LogWarning("재생하려는 이펙트가 설정되어 있지 않음 : " + effectType.ToString());
        }
    }
    void Init()
    {
        Instance = this;

        foreach (EffectKeyValuePair pair in effectList)
        {
            effectDictionary[pair.effectType] = pair.particleSystem;
        }
    }
}
[Serializable]
public struct EffectKeyValuePair
{
    public EffectType effectType;
    public ParticleSystem particleSystem;
}
public enum EffectType
{
    CutlassExplosion,
    BountyTargetDeath,
    ThrowKnifeHit,
}