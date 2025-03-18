using System.Collections;
using UnityEngine;

public class FinalScreenEffectController : MonoBehaviour
{
    public static FinalScreenEffectController Instance { get; private set; } = null;

    [SerializeField] Material finalScreenEffectMaterial;
    public void Awake()
    {
        Instance = this;

        finalScreenEffectMaterial.SetFloat("_Alpha", 0f);
    }
    public void OnDisable()
    {
        finalScreenEffectMaterial.SetFloat("_Alpha", 0f);
    }
    public void OnFinalScreen()
    {
        StartCoroutine(OnFinalScreenCoroutine());
    }
    IEnumerator OnFinalScreenCoroutine()
    {
        float t = 0f;

        while (t < 0.3f)
        {
            t += Time.deltaTime * 0.1f;

            finalScreenEffectMaterial.SetFloat("_Alpha", t);

            yield return null;
        }
        finalScreenEffectMaterial.SetFloat("_Alpha", 0.3f);
    }
}
