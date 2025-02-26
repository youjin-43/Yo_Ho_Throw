using UnityEngine;

public class BulletOnTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        
        //TODO 팀 구분 해야함 
        if (other.CompareTag("Player"))
        {

            PlayerController pc;
            if (pc = other.GetComponent<PlayerController>()){
                pc.OnDamaged(1);

            }
            
        }
        PoolManager.Instance.Push(gameObject);
        
    }

    
}

