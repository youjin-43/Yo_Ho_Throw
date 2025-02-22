using UnityEngine;

public class BulletOnTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        //TODO 팀 구분 해야함 
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Enemy hit");
            PlayerManager pc = other.GetComponent<PlayerManager>();
            pc.OnDamaged(1);
            PoolManager.Instance.Push(gameObject);
            Debug.Log("Anything hit");
        }

    }

    
}

