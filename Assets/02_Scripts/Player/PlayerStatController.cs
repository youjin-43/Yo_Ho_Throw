using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerStatController : MonoBehaviour
{
    public float playerHp;
    public float playerMaxHp;
    
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
        playerHp -= damage;
        if(playerHp <= 0)
        {
            OnDead();
            Debug.Log("Dead");
        }
    }

    public virtual void OnDead()
    {

    }
}
