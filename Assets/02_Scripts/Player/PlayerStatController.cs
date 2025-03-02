using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerStatController : MonoBehaviour
{
    public float playerHp;
    public float playerMaxHp;
    public int bulletCount=10;
    public bool isAlive=true;
    public bool isInRobby=true;
    public float dashCoolTime = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ÆÀ ½Äº°
    public virtual void OnDamaged(float damage)
    {
        if (isInRobby) return;
        
        playerHp -= damage;
        if(playerHp <= 0)
        {
            OnDead();
            
        }
        
    }
    
    public virtual void OnDead()
    {
        Debug.Log("Dead");
    }

    public void FullBullet()
    {
        bulletCount = 10;
    }
}
