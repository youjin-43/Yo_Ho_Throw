using System.Collections;
using UnityEngine;

public class LayerWeightController : StateMachineBehaviour
{
    public float fadeSpeed = 1f;

    //public bool isFadingIn; // true일 경우 weight 증가, false일 경우 weight 감소

    Coroutine[] layerCoroutines = new Coroutine[3] { null, null , null };

    static MonoBehaviour monoBehaviour = null;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monoBehaviour == null) monoBehaviour = animator.GetComponent<MonoBehaviour>();

        if (monoBehaviour == null) Debug.LogWarning("뭔가 잘못됨 !");

        if (layerCoroutines[layerIndex] != null) monoBehaviour.StopCoroutine(layerCoroutines[layerIndex]);

        //layerCoroutines[layerIndex] = monoBehaviour.StartCoroutine(isFadingIn ? FadeInCoroutine(animator, layerIndex) : FadeOutCoroutine(animator, layerIndex));
        layerCoroutines[layerIndex] = monoBehaviour.StartCoroutine(FadeInCoroutine(animator, layerIndex));
    }
    IEnumerator FadeInCoroutine(Animator animator, int layerIndex)
    {
        float t = animator.GetLayerWeight(layerIndex);

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;

            if (animator == null) yield break;

            animator.SetLayerWeight(layerIndex, t);

            yield return null;
        }
        animator.SetLayerWeight(layerIndex, 1);
    }
    IEnumerator FadeOutCoroutine(Animator animator, int layerIndex)
    {
        float t = animator.GetLayerWeight(layerIndex);

        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;

            animator.SetLayerWeight(layerIndex, t);

            yield return null;
        }
        animator.SetLayerWeight(layerIndex, 0);
    }

    public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monoBehaviour == null) monoBehaviour = animator.GetComponent<MonoBehaviour>();

        if (monoBehaviour == null) Debug.LogWarning("뭔가 잘못됨 !");

        if (layerCoroutines[layerIndex] != null) monoBehaviour.StopCoroutine(layerCoroutines[layerIndex]);

        layerCoroutines[layerIndex] = monoBehaviour.StartCoroutine(FadeOutCoroutine(animator, layerIndex));
    }
}
