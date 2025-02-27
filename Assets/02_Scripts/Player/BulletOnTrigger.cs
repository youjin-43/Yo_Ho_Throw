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

        lifeTimeCoroutine = StartCoroutine(AutoDisable());
    }

    private void OnTriggerEnter(Collider other)
    {
         
        
        //TODO 팀 구분 해야함 
        if (other.CompareTag("Player"))
        {
            //TODO 플레이어 맞춘 사운드 이펙트
            PlayerController pc;
            if (pc = other.GetComponent<PlayerController>()){
                pc.OnDamaged(1);

            }
            
        }
        
        if (other.CompareTag("Ground"))
        {
            //TODO 땅 맞춘 사운드 이펙트
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
        // 실행 중인 코루틴이 있으면 정지
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }

        
        PoolManager.Instance.Push(gameObject);
    }

    private void OnDisable()
    {
        // 비활성화될 때 실행 중인 코루틴 정리
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }
    }
}

