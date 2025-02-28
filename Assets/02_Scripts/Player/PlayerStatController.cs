using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerStatController : MonoBehaviour
{
    public float playerHp;
    public float playerMaxHp;
    public bool isAlive=true;
    public bool isInRobby=true;
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
}
