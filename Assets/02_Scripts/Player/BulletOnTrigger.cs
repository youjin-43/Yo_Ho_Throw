using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BulletOnTrigger : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5f;
    private Coroutine lifeTimeCoroutine;

    private void OnEnable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();   
        if (rb != null)
        {
            rb.useGravity = false;
            rb.angularVelocity = Vector3.zero; 
            rb.linearVelocity = Vector3.zero; 
        }
        lifeTimeCoroutine = StartCoroutine(AutoDisable());
    }

    private void OnTriggerEnter(Collider other)
    {

        
        
        if (other.CompareTag("Player"))
        {
            
            PlayerController pc;
            if (pc = other.GetComponent<PlayerController>()){
                pc.OnDamaged();

            }
            PoolManager.Instance.Push(gameObject);
        }
        
        if (other.CompareTag("Ground"))
        {
            
            PoolManager.Instance.Push(gameObject);

        }
        
        
        
    }
    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(lifeTime);
        DeactivateBullet();
    }
    private void DeactivateBullet()
    {
        
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }

        
        PoolManager.Instance.Push(gameObject);
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

