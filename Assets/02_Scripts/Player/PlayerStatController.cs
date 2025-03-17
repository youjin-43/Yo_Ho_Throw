using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Windows;

public class PlayerStatController : MonoBehaviourPun , IDamagable, IOnEventCallback
{
    const int MAX_HP = 3;
    const int MAX_BULLET_COUNT = 5;
    [Header("Player Info")]
    public int playerHp = MAX_HP;
    public int bulletCount = MAX_BULLET_COUNT;
    int coin = 0;
    public bool isAlive = true;
    public bool isInLobby = true;
    protected bool isGameEnd = false;
    public float dashCoolTime = 5f;
    public Animator anim;
    //살아있을때 캐릭터 컨트롤러 offset
    Vector3 centerOffset = new Vector3(0, 0.56f, 0);
    float radiusOffset = 0.28f;
    float heightOffset = 1.12f;
    //죽었을때 캐릭터 컨트롤러 offset
    Vector3 centerDeadOffset = new Vector3(0, 0.12f, -0.6f);
    float radiusDeadOffset = 0.28f;
    float heightDeadOffset = 1.57f;

    private float lastDamageTime = 0f; // 마지막으로 데미지를 받은 시간
    private float healDelay = 5f; // 체력 회복 시작까지의 지연 시간
    private float healInterval = 1f; // 체력 회복 간격

    private Coroutine healingCoroutine;
    private Coroutine bulletReloadCoroutine;

    public GameObject knifeObject;
    public bool isEsc = false;
    
    public int BulletCount
    {
        get => bulletCount;
        set
        {
            if (photonView.IsMine)
            {
                if (bulletCount > value)
                {
                    bulletCount = value;



                    InGameUIManager.Instance.SkillIndicator.RemoveDagger(bulletCount);

                    InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(1, 1f);

                }
                else if (bulletCount < value)
                {
                    InGameUIManager.Instance.SkillIndicator.AddDagger(value - bulletCount);

                    bulletCount = value;
                }
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
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    // Update is called once per frame
    public void Update()
    {
        //Debug.Log(Time.time);   
        if (isAlive && !isInLobby && Time.time - lastDamageTime >= healDelay)
       {
            
            if (healingCoroutine == null)
            {


                photonView.RPC("StartHeal_RPC", RpcTarget.All);
            }
        }

        if(isAlive && !isInLobby && BulletCount<5 )
        {
            if(bulletReloadCoroutine == null)
            {
                if (photonView.IsMine)

                    photonView.RPC("BulletReloadOverTime_RPC", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void StartHeal_RPC()
    {
        if (healingCoroutine == null) 
        {
            healingCoroutine = StartCoroutine(HealOverTime());
        }
    }

    [PunRPC]
    public void SyncLastDamageTime(float damageTime)
    {
        lastDamageTime = damageTime; //모든 클라이언트에서 lastDamageTime 동기화
    }

    [PunRPC]
    public void BulletReloadOverTime_RPC()
    {  
            bulletReloadCoroutine = StartCoroutine(BulletReloadOverTime());
    }
    
    IEnumerator BulletReloadOverTime()
    {
        while (BulletCount < 5)
        {
            yield return new WaitForSeconds(3f);
            BulletCount++;
            if (knifeObject.activeSelf==false)
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

        

        anim.SetTrigger("Hit");
        lastDamageTime = Time.time;

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null; // 체력 회복 중이면 중단
        }
        if (Hp > 0)
        {
            photonView.RPC("PlaySfxAtPosition_RPC", RpcTarget.All, (int)AudioManager.Sfx.PlayerHit, transform.position);
            //AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerHit, transform.position);

        }
        else
        {
            photonView.RPC("PlaySfxAtPosition_RPC", RpcTarget.All, (int)AudioManager.Sfx.PlayerDead, transform.position);

        }
    }
    [PunRPC]
    public void PlaySfxAtPosition_RPC(int sfx, Vector3 position  )
    {
        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerDead, transform.position);
    }

    [PunRPC]
    public void ReceiveDamage(int attackerActorNr, int damage)
    {
        if (isInLobby || !photonView.IsMine) return;
        if(!isAlive) return;
        if (PhotonNetwork.LocalPlayer.ActorNumber != photonView.OwnerActorNr) return;
        
        InGameUIManager.Instance.AddDamage(damage);
        Hp -= damage;

        photonView.RPC("SyncLastDamageTime", RpcTarget.All, Time.time);

        if (Hp <= 0)
        {
            
            photonView.RPC("HandleDeath", RpcTarget.All, attackerActorNr);

        }
        else
        {
            //AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerHit, transform.position);
        }
    }
    [PunRPC]
    public virtual void HandleDeath(int killerActorNr)
    {
        
        if (!photonView.IsMine) return;
        //AudioManager.Instance.PlaySfx(AudioManager.Sfx.PlayerDead);
        gameObject.name += Random.value.ToString();

        // 이동 비활성화
        
        //캐릭터 컨트롤러 오프셋 크기
        CharacterController cc = transform.GetComponent<CharacterController>();
        if (cc == null) Debug.Log("aa");
        

        CursorController.Instance.CursorEnable();
        BattleSystem.Instance.photonView.RPC("RegisterKillRPC", RpcTarget.All, killerActorNr, photonView.OwnerActorNr);
        StartCoroutine(ApplyGravityAfterDeath());

        photonView.RPC("PlayDeathAnimation_RPC", RpcTarget.All);

    }
    
    private IEnumerator ApplyGravityAfterDeath()
    {
        PlayerController pc = GetComponent<PlayerController>();

        while (!transform.GetComponent<PlayerController>().Grounded)
        {
            Debug.Log("여기");
            pc._controller.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));
            yield return null;
        }
       

        isAlive = false;
        photonView.RPC("PlayDeathAnimation_RPC", RpcTarget.All);
        
    }
    [PunRPC]
    public void PlayDeathAnimation_RPC()
    {
        StartCoroutine(PlayDeathAnimationCorutine());
        
    }
    IEnumerator PlayDeathAnimationCorutine()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            anim.SetTrigger("Dead"); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator HealOverTime()
    {
        //Debug.Log("회복");
        while (Hp < MAX_HP)
        {

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
        if (!photonView.IsMine) transform.gameObject.layer = LayerMask.NameToLayer("Ground");
        anim.Rebind();
        anim.Update(0f);
        if (!photonView.IsMine) return;
        //콜라이더 오프셋

        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerRevive, transform.position);

        playerHp = MAX_HP;
        InGameUIManager.Instance.StatusIndicator.SetHealth(playerHp);
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
        if (isSettingColor)
        {
            photonView.RPC("RespawnDefaultColorSetting", RpcTarget.All);

            isSettingColor = false;
        }
        CursorController.Instance.CursorDisable();
    }

    [PunRPC]
    public void GameEndPlayer()
    {
        anim.speed = 0;
        isGameEnd = true;
    }
    [PunRPC]
    public void GameStartPlayer()
    {
        anim.speed = 1;
        isGameEnd = false;
    }

    
    [PunRPC]
    public void IsKnifeOn(bool onoff)
    {
        //if (!photonView.IsMine) return;

        if (onoff)
        {
            knifeObject.gameObject.SetActive(true);
        }
        else
        {
            knifeObject.gameObject.SetActive(false);
        }
    }

    bool isSettingColor = false;
    bool isStealthMaterial = false;
    int beforeColorSetting = 0; // 0 : default, 1 : bounty

    [SerializeField] Material defaultColorMaterial;
    [SerializeField] Material bountyColorMaterial;
    [SerializeField] Material stealthMaterial;

    [PunRPC]
    public void DefaultColorSetting()
    {
        beforeColorSetting = 0;

        if (!isStealthMaterial)
        {
            transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = defaultColorMaterial;
        }
    }
    [PunRPC]
    public void RespawnDefaultColorSetting()
    {
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = defaultColorMaterial;
    }
    [PunRPC]
    public void BountyColorSetting()
    {
        isSettingColor = false;

        beforeColorSetting = 1;

        if (!isStealthMaterial)
        {
            transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = bountyColorMaterial;
        }
    }
    [PunRPC]
    public void RespawnColorSetting()
    {
        isSettingColor = true;
    }
    Coroutine stealthCoroutine = null;

    [PunRPC]
    public void StealthSetting()
    {
        stealthCoroutine = StartCoroutine(StealthCoroutine());

        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = stealthMaterial;
    }
    [PunRPC]
    public void ExposeSetting()
    {
        if (isStealthMaterial)
        {
            isStealthMaterial = false;

            if (stealthCoroutine != null)
            {
                StopCoroutine(stealthCoroutine);
            }
            eyePatch.material = eyePatchDefaultMaterial;

            cutlass.material = cutlassDefaultMaterial;

            transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = beforeColorSetting == 0 ? defaultColorMaterial : bountyColorMaterial;
        }
    }
    [SerializeField] MeshRenderer eyePatch;
    [SerializeField] Material eyePatchDefaultMaterial;
    [SerializeField] MeshRenderer cutlass;
    [SerializeField] Material cutlassDefaultMaterial;
    IEnumerator StealthCoroutine()
    {
        isStealthMaterial = true;

        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = stealthMaterial;

        eyePatch.material = stealthMaterial;
        cutlass.material = stealthMaterial;

        yield return new WaitForSeconds(10f);

        isStealthMaterial = false;

        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = beforeColorSetting == 0 ? defaultColorMaterial : bountyColorMaterial;

        eyePatch.material = eyePatchDefaultMaterial;
        cutlass.material = cutlassDefaultMaterial;
    }

    [PunRPC]
    public void FullKnife()
    {
        BulletCount = 5;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.EditClientCoin:
                EditClientCoin((object[])photonEvent.CustomData); break;

            case RaiseEventCode.EditHostCoin:
                EditHostCoin((int)photonEvent.CustomData); break;
        }
    }
    public void AddCoin(int _coin)
    {
        coin += _coin;

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.EditClientCoin,
            new object[] { _coin, photonView.OwnerActorNr },
            new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache },
            SendOptions.SendReliable
            );

        InGameUIManager.Instance.SetGoldCoin(coin, photonView.OwnerActorNr);
    }
    void EditClientCoin(object[] data)
    {
        Debug.Log("-EditClientCoin-");
        Debug.Log("나 : " + PhotonNetwork.LocalPlayer.ActorNumber.ToString() + " / 받은 액터 : " + (int)data[1]);
        Debug.Log("받은 점수 : " + ((int)data[0]).ToString());

        if (PhotonNetwork.LocalPlayer.ActorNumber != (int)data[1]) return;

        int coin = (int)data[0];

        PlayerSpawnManager.Instance.coin = coin;

        this.coin = coin;
    }
    void EditHostCoin(int _coin)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        coin = _coin;
    }
    public void DeleteCoin(int _coin)
    {
        if (coin - _coin >= 0) coin -= _coin;

        PhotonNetwork.RaiseEvent(
            (byte)RaiseEventCode.EditHostCoin,
            _coin,
            new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache },
            SendOptions.SendReliable
            );

        PlayerSpawnManager.Instance.coin = coin;

        InGameUIManager.Instance.SetGoldCoin(coin, photonView.OwnerActorNr);
    }
}
