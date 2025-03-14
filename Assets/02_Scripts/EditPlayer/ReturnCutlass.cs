using ExitGames.Client.Photon;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ReturnCutlass : MonoBehaviour
{
    Coroutine lifeTimeCoroutine = null;
    bool inPool = true;
    [SerializeField] float lifeTime;
    [SerializeField] ExplosionCutlass explosionCutlass = null;

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
        if (other.gameObject.GetInstanceID() ==
            transform.parent.gameObject.GetInstanceID()) return;

        explosionCutlass?.Explosion();

        DeactivateKnife();
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

            PoolManager.Instance.Push(transform.parent.parent.gameObject);

            transform.parent.parent.gameObject.SetActive(false);
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
