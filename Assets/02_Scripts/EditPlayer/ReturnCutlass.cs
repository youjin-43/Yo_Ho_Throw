using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine;

public class ReturnCutlass : MonoBehaviour
{
    Coroutine lifeTimeCoroutine = null;
    bool inPool = true;
    [SerializeField] float lifeTime;

    private void OnEnable()
    {
        inPool = false;

        lifeTimeCoroutine = StartCoroutine(AutoDisable());
    }
    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(lifeTime);

        DeactivateKnife();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            DeactivateKnife();
        }
    }
    public void DeactivateKnife()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);

            lifeTimeCoroutine = null;
        }
        Push();
    }
    void Push()
    {
        if (inPool == false)
        {
            inPool = true;

            PoolManager.Instance.Push(transform.parent.gameObject);
        }
    }
    private void OnDisable()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);

            lifeTimeCoroutine = null;
        }
    }
}
