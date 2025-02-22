using UnityEngine;

public class BulletOnTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        
        //TODO 팀 구분 해야함 
        if (other.CompareTag("Player"))
        {
            
            PlayerManager pc;
            if (pc = other.GetComponent<PlayerManager>()){
                pc.OnDamaged(1);

            }
            
        }
        PoolManager.Instance.Push(gameObject);
        
    }

    
}

