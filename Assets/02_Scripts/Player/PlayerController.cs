using Photon.Pun;
using StarterAssets;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;


public class PlayerController : ThirdPersonController
{
    //PhotonView pv;
    private PhotonTransformView photonTransformView;

    [Header("Online")]
    public bool online = true;

    [Header("State")]
    public bool canDash = true;
    bool isDashing = false;

    private StarterAssetsInputs input;
    public bool isAttacking = false;

    [Header("Bullet")]
    public float bulletRange = 100f;
    public GameObject bulletPrefab;
    public GameObject explosionBulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    public float bulletArc = 5f;

    [Header("Melee Attack")]
    public GameObject meleeAttackColliderObject;

    //[Header("Aim")]
    //public Transform cameraTransform;
    //public CinemachineVirtualCamera aimCam;
    [SerializeField]
    private LayerMask targetLayer;

    private Vector3 targetPosition = Vector3.zero;
    private int maxBulletCount;
    [SerializeField]
    private Transform cameraTransform;

    bool isKnifeConsumed = true;
    bool isExplosionBuff = false;

    void Start()
    {
        base.Start();
        cameraTransform = Camera.main.transform;
        photonTransformView = GetComponent<PhotonTransformView>();

        if (InGameUIManager.Instance != null)
            InGameUIManager.Instance.SetPlayerID(PhotonNetwork.NickName);
    }
    void Update()
    {

        if (online && !photonView.IsMine) return;
        
        if (isGameEnd) return;

        base.Update();
        
        if (!isAlive) return;

        if(GameManager.Instance.isPlayerStop) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Grounded   && canDash)
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) return; // 현재 정지하고 있으면 대쉬 못하도록 

            //anim.SetTrigger("Dash");
            //Dash();
            //StartCoroutine(DashCooltime()); // 쿨타임 적용

            Optimal_Dash();
        }

        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && BulletCount > 0)
        {
            anim.SetTrigger("Shoot");
            
            StartCoroutine(AttackCoroutine(1f));
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)&& BulletCount > 0)
        {
            anim.SetTrigger("Melee Attack");
            //TODO 석진 근접 공격 휘두르는 사운드 
            StartCoroutine(AttackCoroutine(0.6f));
        }
    }

    void FixedUpdate()
    {
        if (online && !photonView.IsMine) return;

        if (isGameEnd) return;

        if (GameManager.Instance.isPlayerStop) return;

        base.FixedUpdate();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        anim = GetComponent<Animator>();
        anim.Update(0f);
    }

    IEnumerator AttackCoroutine(float time)
    {
        isAttacking = true; 
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    //IEnumerator DashCooltime()
    //{
    //    canDash = false;
    //    yield return new WaitForSeconds(dashCoolTime);
    //    canDash = true;
    //}

    #region Throw
    public void ThrowProjectile()
    {
        if (!isKnifeConsumed)
        {
            ThrowCutlass();
        }
        else if (BulletCount <= 0) return;
        else if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            ThrowCutlass();
        }
    }

    void ThrowCutlass()
    {
        if (!isInLobby) BulletCount -= isKnifeConsumed ? 1 : 0;

        if (BulletCount == 0)
            photonView.RPC("IsKnifeOn", RpcTarget.All, false);

        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        Vector3 throwDirection = ((cameraTransform.forward * bulletRange + cameraTransform.position + Vector3.up * 3) - bulletSpawnPoint.position).normalized;

        if (online && photonView.IsMine)
        {
            photonView.RPC("Throw_RPC", RpcTarget.All, throwDirection, PhotonNetwork.LocalPlayer.ActorNumber, isExplosionBuff);
        }
        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerAttack, transform.position);


        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerMelee,transform.position);

        
    }

    [PunRPC]
    void Throw_RPC(Vector3 throwDirection, int attackerActorNr, bool isExplosionBuff)
    {
        // 칼 오브젝트 생성 

        GameObject projectile =
            PoolManager.Instance.Pop(isExplosionBuff ? explosionBulletPrefab : bulletPrefab);

        projectile.transform.GetChild(0).GetComponent<Collider>().enabled = true;

        if (projectile == null) return;

        projectile.GetComponentInChildren<Cutlass>().Setting(attackerActorNr, bulletSpawnPoint.position);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;

            Quaternion rotationOffset = Quaternion.Euler(-35f, 0, 0);

            projectile.transform.rotation = Quaternion.LookRotation(throwDirection) * rotationOffset;

            rb.linearVelocity = throwDirection * bulletSpeed;
        }
    }
    #endregion

    public void LayerReset()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 1);
    }

    public void Optimal_Dash()
    {
        if (!canDash || isDashing) return; // 현재 쿨이거나 대쉬중이면 뇹

        if (!isInLobby) InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(2, dashCoolTime);

        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash"); // 애니메이션 실행

        float input_Y = Input.GetAxisRaw("Vertical");
        float input_X = input_Y == -1 ? 0 : Input.GetAxisRaw("Horizontal");

        Vector3 direction = transform.forward * input_Y + transform.right * input_X;

        anim.SetFloat("HorizontalRaw", Mathf.Round(input_X));
        anim.SetFloat("VerticalRaw", Mathf.Round(input_Y));

        StartCoroutine(DashMovement_RPC(direction.normalized));
        //canDash = true;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
    }

    public void Dash()
    {
        anim.SetLayerWeight(1, 0);
        if (photonTransformView != null)
        {
            photonTransformView.enabled = false;
        }

        //float input_Y = _input.move.y;
        //float input_X = input_Y == -1 ? 0 : _input.move.x;
        float input_Y = Input.GetAxisRaw("Vertical");
        float input_X = input_Y == -1 ? 0 : Input.GetAxisRaw("Horizontal");

        if (online && photonView.IsMine)
        {   
            if(!isInLobby) InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(2, 5f);
            photonView.RPC("Dash_RPC", RpcTarget.All, input_X, input_Y);
        }
        else
            Dash_RPC(input_X, input_Y);
        //TODO 석진 플레이어 대쉬 소리
        //AudioManager.Instance.PlaySfx(AudioManager.Sfx.Player);
    }

    [PunRPC]
    void Dash_RPC(float horizontalInput, float verticalInput)
    {
        //StartCoroutine(StartAnimationCoroutine("Dash", 0.1638f, true, 1, 0.1f));

        

        Vector3 dashDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        dashDirection.Normalize();

        anim.SetFloat("HorizontalRaw", Mathf.Round(horizontalInput));
        anim.SetFloat("VerticalRaw", Mathf.Round(verticalInput));

        StartCoroutine(DashMovement_RPC(dashDirection));
    }

    IEnumerator DashMovement_RPC(Vector3 direction)
    {
        float dashDistance = 3f;
        float dashTime = 0.1638f;
        float elapsedTime = 0f;
        Vector3 velocity = direction * (dashDistance / dashTime);

        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerDash, transform.position);
        
        while (elapsedTime < dashTime)
        {
            _controller.Move(velocity * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    
    public void MeleeAttack()
    {
        if (!isInLobby && photonView.IsMine) InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(0, 0.6f);

        if (online && photonView.IsMine)
            photonView.RPC("MeleeAttack_RPC", RpcTarget.All);

        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerAttack, transform.position);
        AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.PlayerMelee, transform.position);
    }

    [PunRPC]
    void MeleeAttack_RPC()
    {
        //if (!photonView.IsMine) return;
        ExposeSetting();
        StartCoroutine(EnableCollider_RPC(meleeAttackColliderObject, 0.4f));
    }

    private IEnumerator EnableCollider_RPC(GameObject _meleeAttackColliderObject, float time)
    {
        _meleeAttackColliderObject.SetActive(true);
        yield return new WaitForSeconds(time);
        _meleeAttackColliderObject.SetActive(false);
    }

    Coroutine explosionBuffCoroutine = null;
    Coroutine infinityKnifeBuffCoroutine = null;
    public void GetExplosionBuff()
    {
        if (explosionBuffCoroutine != null) StopCoroutine(explosionBuffCoroutine);

        explosionBuffCoroutine = StartCoroutine(ExplosionBuffCoroutine());
    }
    IEnumerator ExplosionBuffCoroutine()
    {
        isExplosionBuff = true;

        yield return new WaitForSeconds(10f);

        isExplosionBuff = false;
    }
    public void GetInfinityKnifeBuff()
    {
        if (infinityKnifeBuffCoroutine != null) StopCoroutine(infinityKnifeBuffCoroutine);

        infinityKnifeBuffCoroutine = StartCoroutine(InfinityKnifeBuffCoroutine());
    }
    IEnumerator InfinityKnifeBuffCoroutine()
    {
        isKnifeConsumed = false;

        yield return new WaitForSeconds(10f);

        isKnifeConsumed = true;
    }
    void ClearOnDeath()
    {
        if (explosionBuffCoroutine != null) StopCoroutine(explosionBuffCoroutine);

        if (infinityKnifeBuffCoroutine != null) StopCoroutine(infinityKnifeBuffCoroutine);

        isExplosionBuff = false;
    }
    [PunRPC]
    public override void HandleDeath(int killerActorNr)
    {
        ClearOnDeath();

        base.HandleDeath(killerActorNr);
    }
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSpeed = newSensitivity;
    }
}
