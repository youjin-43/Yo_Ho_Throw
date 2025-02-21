using UnityEngine;

public class BulletOnCollider : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        //TODO 팀 구분 해야함 
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager pc = collision.gameObject.GetComponent<PlayerManager>();
            pc.OnDamaged(1);
        }

        PoolManager.Instance.Push(gameObject);
        
    }
}
