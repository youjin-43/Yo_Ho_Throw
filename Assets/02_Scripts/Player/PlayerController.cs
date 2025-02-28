using Photon.Pun;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : ThirdPersonController
{
    PhotonView pv;

    [Header("Online")]
    public bool online = true;

    [Header("Bullet")]
    private StarterAssetsInputs input;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    public float bulletArc = 5f;

    [Header("Melee Attack")]
    public GameObject meleeAttackColliderObject;

    [Header("Aim")]
    public Transform cameraTransform;
    public CinemachineVirtualCamera aimCam;
    [SerializeField]
    private LayerMask targetLayer;

    private Vector3 targetPosition = Vector3.zero;
    private int maxBulletCount;
    [SerializeField]
    private int bulletCount;
    private Animator anim;

    void Start()
    {
        base.Start();
        input = GetComponent<StarterAssetsInputs>();

        maxBulletCount = 10;
        bulletCount = maxBulletCount;
        anim = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (online && !pv.IsMine) return;
        base.Update();

        LookSameCameraDirection();

        if (input.aim) aimCam.gameObject.SetActive(true);
        else aimCam.gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.F) && bulletCount > 0)
        {
            anim.SetTrigger("Shoot");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger("Dash");
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            MeleeAttack();
        }
    }

    void FixedUpdate()
    {
        if (online && !pv.IsMine) return;
        base.FixedUpdate();
    }

    // 🔥 [투사체 공격] - 네트워크 RPC 적용
    [PunRPC]
    void ThrowProjectile_RPC()
    {
        if (bulletCount == 0) return;

        StartCoroutine(StartAnimationCoroutine("Shoot", 0.24f));

        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            bulletCount--;
            GameObject projectile = PoolManager.Instance.Pop(bulletPrefab);
            if (projectile == null) return;
            projectile.transform.position = bulletSpawnPoint.position;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                Vector3 throwDirection = (targetPosition - bulletSpawnPoint.position).normalized;
                Quaternion rotationOffset = Quaternion.Euler(90, 0, 0);
                projectile.transform.rotation = Quaternion.LookRotation(throwDirection) * rotationOffset;
                rb.linearVelocity = throwDirection * bulletSpeed;
            }
        }
    }

    public void ThrowProjectile()
    {
        if (online && pv.IsMine)
            pv.RPC("ThrowProjectile_RPC", RpcTarget.All);
        else
            ThrowProjectile_RPC();
    }

    // 🔥 [애니메이션 실행] - 네트워크 동기화
    IEnumerator StartAnimationCoroutine(string _animName, float _frame, bool _layerLerp = false, int _layerIndex = 0, float _layerWeight = 1)
    {
        anim.SetTrigger(_animName);
        anim.SetLayerWeight(_layerIndex, _layerWeight);
        yield return new WaitForSeconds(_frame);
        anim.SetTrigger(_animName);

        if (_layerLerp)
            StartCoroutine(SmoothLayerReset(_layerIndex));
        else
            LayerReset();
    }

    public void LayerReset()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 1);
    }

    private IEnumerator SmoothLayerReset(int _layerIndex)
    {
        float elapsedTime = 0f;
        float blendTime = 0.3f;
        float currentWeight = anim.GetLayerWeight(_layerIndex);

        while (elapsedTime < blendTime)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(currentWeight, 1, elapsedTime / blendTime);
            anim.SetLayerWeight(1, newWeight);
            yield return null;
        }
        anim.SetLayerWeight(1, 1);
    }

    // 🔥 [카메라 방향을 따라 플레이어 회전]
    void LookSameCameraDirection()
    {
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;
        Vector3 previousTargetPosition = targetPosition;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 100f, targetLayer))
        {
            if (Vector3.Distance(previousTargetPosition, hit.point) > 0.1f)
            {
                targetPosition = Vector3.Lerp(previousTargetPosition, hit.point, Time.deltaTime * 100f);
            }
        }
        else
        {
            Vector3 newTargetPosition = camTransform.position + camTransform.forward * 100f;
            targetPosition = Vector3.Lerp(previousTargetPosition, newTargetPosition, Time.deltaTime * 100f);
        }

        Vector3 targetAim = targetPosition;
        targetAim.y = transform.position.y;
        Vector3 aimDir = (targetAim - transform.position).normalized;

        transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * 30f);
    }

    // 🔥 [대시 기능] - 네트워크 적용
    public void Dash()
    {
        if (online && pv.IsMine)
            pv.RPC("Dash_RPC", RpcTarget.All);
        else
            Dash_RPC();
    }

    [PunRPC]
    void Dash_RPC()
    {
        StartCoroutine(StartAnimationCoroutine("Dash", 0.467f, true, 1, 0.1f));

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dashDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        dashDirection.Normalize();

        anim.SetFloat("HorizontalRaw", Mathf.Round(horizontalInput));
        anim.SetFloat("VerticalRaw", Mathf.Round(verticalInput));

        StartCoroutine(DashMovement_RPC(dashDirection));
    }

    [PunRPC]
    IEnumerator DashMovement_RPC(Vector3 direction)
    {
        float dashDistance = 3f;
        float dashTime = 0.4915f;
        float elapsedTime = 0f;
        Vector3 velocity = direction * (dashDistance / dashTime);

        while (elapsedTime < dashTime)
        {
            _controller.Move(velocity * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // 🔥 [근접 공격] - 네트워크 적용
    public void MeleeAttack()
    {
        if (online && pv.IsMine)
            pv.RPC("MeleeAttack_RPC", RpcTarget.All);
        else
            MeleeAttack_RPC();
    }

    [PunRPC]
    void MeleeAttack_RPC()
    {
        StartCoroutine(StartAnimationCoroutine("Melee Attack", 0.833f));
    }

    [PunRPC]
    void EnableMeleeAttackCollider_RPC()
    {
        StartCoroutine(EnableCollider_RPC(meleeAttackColliderObject, 0.4f));
    }

    private IEnumerator EnableCollider_RPC(GameObject _meleeAttackColliderObject, float time)
    {
        _meleeAttackColliderObject.SetActive(true);
        yield return new WaitForSeconds(time);
        _meleeAttackColliderObject.SetActive(false);
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);
        StartCoroutine(StartAnimationCoroutine("Hit", 0.667f));
    }
}
