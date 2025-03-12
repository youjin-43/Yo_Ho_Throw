using UnityEngine;
using VInspector;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; } = null;

    [SerializeField] SerializedDictionary<EffectType, ParticleSystem> effectList;

    private void Awake()
    {
        Instance = this;
    }
    public void Play(Vector3 pos, EffectType effectType)
    {
        if (!effectList.ContainsKey(effectType))
            Debug.LogWarning("재생하려는 이펙트가 설정되어 있지 않음 : " + effectType.ToString());

        effectList[effectType].transform.position = pos;

        effectList[effectType].Play();
    }
}

public enum EffectType
{
    CutlassExplosion
}