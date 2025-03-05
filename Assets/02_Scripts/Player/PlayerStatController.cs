using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerStatController : MonoBehaviour
{
    public float playerHp;
    public float playerMaxHp;
    public int bulletCount = 10;
    public bool isAlive = true;
    public bool isInRobby = true;
    public float dashCoolTime = 5f;

    private float lastDamageTime = 0f; // 마지막으로 데미지를 받은 시간
    private float healDelay = 5f; // 체력 회복 시작까지의 지연 시간
    private float healInterval = 1f; // 체력 회복 간격

    private Coroutine healingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        //Debug.Log(Time.time);   
        if (isAlive && !isInRobby && Time.time - lastDamageTime >= healDelay)
        {
            Debug.Log("5ch");
            if (healingCoroutine == null)
            {

                healingCoroutine = StartCoroutine(HealOverTime());
            }
        }
    }

    //팀 식별
    public virtual void OnDamaged(float damage)
    {
        if (isInRobby) return;

        playerHp -= damage;
        lastDamageTime = Time.time;

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null; // 체력 회복 중이면 중단
        }

        if (playerHp <= 0)
        {
            OnDead();
            isAlive = false;
        }

    }
    private IEnumerator HealOverTime()
    {
        Debug.Log("회복");
        while (playerHp < playerMaxHp)
        {
            playerHp += 1; // 체력 1씩 회복
            playerHp = Mathf.Min(playerHp, playerMaxHp); // 최대 체력 초과 방지
            yield return new WaitForSeconds(healInterval);
        }
        healingCoroutine = null; // 체력 다 차면 종료
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
