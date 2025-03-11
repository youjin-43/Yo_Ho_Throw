using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Windows;

public class PlayerStatController : MonoBehaviourPun , IDamagable
{
    const int MAX_HP = 3;
    const int MAX_BULLET_COUNT = 5;
    [Header("Player Info")]
    public int playerHp = MAX_HP;
    public int bulletCount = MAX_BULLET_COUNT;
    public int coin = 0;
    public bool isAlive = true;
    public bool isInLobby = true;
    public bool isGameEnd = false;
    public float dashCoolTime = 5f;
    public Animator anim;

    private float lastDamageTime = 0f; // 마지막으로 데미지를 받은 시간
    private float healDelay = 5f; // 체력 회복 시작까지의 지연 시간
    private float healInterval = 1f; // 체력 회복 간격

    private Coroutine healingCoroutine;
    private Coroutine bulletReloadCoroutine;

    public GameObject knifeObject;
    public int BulletCount
    {
        get => bulletCount;
        set
        {
            if (bulletCount > value)
            {
                while (bulletCount > value) 
                {
                    //InGameUIManager.Instance.SkillIndicator.RemoveDagger();
                    Debug.Log("칼씀");
                    bulletCount--;
                }
            }
            else if (bulletCount < value)
            {
                InGameUIManager.Instance.SkillIndicator.AddDagger(value - bulletCount);
                Debug.Log("칼 얻음");
                bulletCount = value;
            }
        }
    }

    int Hp
    {
        get => playerHp;

        set
        {
            playerHp = value;

            InGameUIManager.Instance.StatusIndicator.SetHealth(playerHp);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        //Debug.Log(Time.time);   
        if (isAlive && !isInLobby && Time.time - lastDamageTime >= healDelay)
       {
            
            if (healingCoroutine == null)
            {

                healingCoroutine = StartCoroutine(HealOverTime());
            }
        }

        if(isAlive && !isInLobby && BulletCount<5 )
        {
            if(bulletReloadCoroutine == null)
            {
                bulletReloadCoroutine = StartCoroutine(BulletReloadOverTime());
            }
        }
    }
    
    IEnumerator BulletReloadOverTime()
    {
        while (BulletCount < 5)
        {
            yield return new WaitForSeconds(3f);
            BulletCount++;
            if (BulletCount == 1)
            {
                if(photonView.IsMine) 
                    photonView.RPC("IsKnifeOn", RpcTarget.All,true);
            }
            
        }
        bulletReloadCoroutine = null;   
    }

    //애니메이션,힐 코루틴 용도
    public virtual void OnDamagedAnim()
    {
        if (isInLobby) return;

        //AudioManager.Instance.PlaySfx(AudioManager.Sfx.PlayerHit);

        anim.SetTrigger("Hit");
        lastDamageTime = Time.time;

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null; // 체력 회복 중이면 중단
        }

    }
    [PunRPC]
    public void ReceiveDamage(int attackerActorNr, int damage)
    {
        if (isInLobby || !photonView.IsMine) return;
        if(!isAlive) return;
        if (PhotonNetwork.LocalPlayer.ActorNumber != photonView.OwnerActorNr) return;
        
        InGameUIManager.Instance.StatusIndicator.AddDamage(damage);
        Hp -= damage;

        if (Hp <= 0)
        {
            
            photonView.RPC("HandleDeath", RpcTarget.All, attackerActorNr);
        }
    }
    [PunRPC]
    public virtual void HandleDeath(int killerActorNr)
    {
        anim.SetTrigger("Dead");
        if (!photonView.IsMine) return;
        //AudioManager.Instance.PlaySfx(AudioManager.Sfx.PlayerDead);
        gameObject.name += Random.value.ToString();

        // 이동 비활성화
        isAlive = false;

        CursorController.Instance.CursorEnable();
        BattleSystem.Instance.photonView.RPC("RegisterKillRPC", RpcTarget.All, killerActorNr, photonView.OwnerActorNr);
    }

    [PunRPC]
    private IEnumerator HealOverTime()
    {
        //Debug.Log("회복");
        while (Hp < MAX_HP)
        { Debug.Log("체력 회복");

            InGameUIManager.Instance.AddHealth(1);
            playerHp += 1; // 체력 1씩 회복
            playerHp = Mathf.Min(Hp, MAX_HP); // 최대 체력 초과 방지
            yield return new WaitForSeconds(healInterval);
        }
        healingCoroutine = null; // 체력 다 차면 종료
    }
    
    
    [PunRPC]
    public void OnInLobby()
    {
        isInLobby = true;
    }
    [PunRPC]
    public void OnOutLobby()
    {
        isInLobby = false;
    }

    [PunRPC]
    public void InitPlayer()
    {
        //TODO 석진 플레이어 다시 살아나는 소리

        anim.Rebind();
        anim.Update(0f);
        if (!photonView.IsMine) return;

        playerHp = MAX_HP;
        isAlive = true;

        BulletCount = 5;

        //카메라와 맞는 방향으로 회전
        Transform camTransform = Camera.main.transform;
        Vector3 cameraForward = camTransform.forward;
        cameraForward.y = 0; 
        transform.rotation = Quaternion.LookRotation(cameraForward);
        if(photonView.IsMine)
            photonView.RPC("IsKnifeOn", RpcTarget.All, true); 
        
        // 현상금 타겟으로써 죽었을 경우 플레이어 메테리얼 기존 것으로 설정
        if (isSettingColor) transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = defaultColorMaterial;

        CursorController.Instance.CursorDisable();
    }

    [PunRPC]
    public void GameEndPlayer()
    {
        isGameEnd = true;

        CursorController.Instance.CursorEnable();
    }
    [PunRPC]
    public void GameStartPlayer()
    {
        isGameEnd=false;
    }

    
    [PunRPC]
    public void IsKnifeOn(bool onoff)
    {
        //if (!photonView.IsMine) return;

        if (onoff)
        {
            knifeObject.gameObject.SetActive(true);
            Debug.Log("나이프 켜짐");
        }
        else
        {
            knifeObject.gameObject.SetActive(false);
            Debug.Log("나이프 꺼짐");
        }
    }

    bool isSettingColor = false;
    int beforeColorSetting = 0; // 0 : default, 1 : bounty

    [SerializeField] Material defaultColorMaterial;
    [SerializeField] Material bountyColorMaterial;
    [SerializeField] Material stealthMaterial;

    [PunRPC]
    public void DefaultColorSetting()
    {
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = defaultColorMaterial;
    }
    [PunRPC]
    public void BountyColorSetting()
    {
        isSettingColor = false;
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = bountyColorMaterial;
    }
    [PunRPC]
    public void RespawnColorSetting()
    {
        isSettingColor = true;
    }
    public void StealthSetting()
    {
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = stealthMaterial;
    }
    public void ExposeSetting()
    {

    }
    [PunRPC]
    public void AddCoin(int _coin)
    {
        coin += _coin;
    }
    [PunRPC]
    public void DeleteCoin(int _coin)
    {
        if(coin-_coin>=0)
            coin -= _coin;
    }

}
