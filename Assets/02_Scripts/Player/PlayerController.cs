using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : ThirdPersonController
{
    PhotonView pv;

    private StarterAssetsInputs input;
    public GameObject bulletPrefab; // 투사체 프리펩
    public Transform bulletSpawnPoint; // 투사체 생성 위치
    public float bulletSpeed = 10f; // 투사체 속도
    public float bulletArc = 5f; // 투사체가 포물선을 그릴 때 사용 하는 값
    public Transform cameraTransform;
    [Header("Aim")]
    public CinemachineVirtualCamera aimCam;
    
    
    [SerializeField]
    private LayerMask targetLayer;
    
    private Vector3 targetPosition = Vector3.zero;
    private int maxBulletCount;
    [SerializeField]
    private int bulletCount;

    private Animator anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        input = GetComponent<StarterAssetsInputs>();
        
        maxBulletCount = 10;
        bulletCount = maxBulletCount;
        anim = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
    }

    
    // Update is called once per frame
    void Update()
    {   //커밋전삭제
        if (!pv.IsMine) 
            return;

        base.Update();

            LookSameCameraDirection();

            //줌 할때의 카메라를 활성화 시킴
            if (input.aim)
            {
                aimCam.gameObject.SetActive(true);
            }
            else
            {
                aimCam.gameObject.SetActive(false);
            }

            //F키가 투척
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (bulletCount != 0)
                    anim.SetTrigger("Shoot");

            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                anim.SetTrigger("Dash");
                Dash();
            }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //anim.SetTrigger("Melee Attack");
            MeleeAttack();
        }
        



    }
    public void FixedUpdate()
    {
        //커밋전삭제
        if (!pv.IsMine)
            return;

        base.FixedUpdate();
        
    }

    

    /// <summary>
    /// 투사체를 생성하여 던지는 함수 
    /// 플레이어 애니메이션의 throw attack에서 호출됨
    /// </summary>
    [PunRPC]
    void ThrowProjectile_RPC()
    {
        if (bulletCount == 0)
            return;

        StartCoroutine(StartAnimationCoroutine("Shoot", 0.24f));

        if (bulletPrefab != null && bulletSpawnPoint != null )
        {
            bulletCount -= 1;
            GameObject projectile = PoolManager.Instance.Pop(bulletPrefab);
            if(projectile == null)
            {
                Debug.Log("dd");
            }
            projectile.transform.position = bulletSpawnPoint.position;
            
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.useGravity = false;

                Vector3 throwDirection = (targetPosition - bulletSpawnPoint.position).normalized; //포물선 일때 Vector3.up * bulletArc;
                Quaternion rotationOffset = Quaternion.Euler(90, 0, 0); 
                projectile.transform.rotation = Quaternion.LookRotation(throwDirection) * rotationOffset;
                
                rb.linearVelocity = throwDirection*bulletSpeed;
            }
            
        }
    }

    public void ThrowProjectile()
    {   //커밋전삭제
        pv.RPC("ThrowProjectile_RPC", RpcTarget.All);
        //ThrowProjectile_RPC();
    }

    /// <summary>
    /// 트리거이름과 애니메이션 길이를 넣어 재생. 다른 레이어의 가중치를 없애고 싶을때 3,4번째
    /// 해당 애니메이션이 실행 될 때 카메라를 따라가지 않는다면 5번째 인자에 false
    /// </summary>
    /// <param name="animName"></param> 
    /// <param name="frame"></param>
    /// <param name="layerWeight"></param>
    /// <param name="targetLayer"></param>
    /// <returns></returns>
    IEnumerator StartAnimationCoroutine(string _animName , float _frame , int _layerIndex = 0 , float _layerWeight = 1)
    {   
        
        anim.SetTrigger(_animName);  
        anim.SetLayerWeight(_layerIndex, _layerWeight);
        yield return new WaitForSeconds(_frame);
        anim.SetTrigger(_animName);
        LayerReset();
    }

    public void LayerReset()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetLayerWeight(0, 1);
    }
    /// <summary>
    /// 캐릭터가 카메라가 바라보는 곳을 레이케스트 힛 된 곳으로 바꾸는 함수
    /// </summary>
    void LookSameCameraDirection()
    {
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        Vector3 targetAim;
        Vector3 aimDir = Vector3.zero;
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

        
        targetAim = targetPosition;
        targetAim.y = transform.position.y;
        aimDir = (targetAim - transform.position).normalized;

        
        transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * 30f);

        
    }

    public void Dash()
    {
        StartCoroutine(StartAnimationCoroutine("Dash", 0.467f, 1,0.1f));


        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dashDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        dashDirection.Normalize();

        anim.SetFloat("HorizontalRaw", Mathf.Round(horizontalInput));
        anim.SetFloat("VerticalRaw", Mathf.Round(verticalInput));
        StartCoroutine(DashMovement(dashDirection));
    }

    private IEnumerator DashMovement(Vector3 direction)
    {
        float dashDistance = 3f;   // 대시 거리
        float dashTime = 0.4915f;   // 대시 지속 시간
        float elapsedTime = 0f;

        // 플레이어가 바라보는 방향으로 대시하는 속도 계산
        Vector3 velocity = direction * (dashDistance / dashTime);

        while (elapsedTime < dashTime)
        {
            _controller.Move(velocity * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    public void MeleeAttack()
    {
        StartCoroutine(StartAnimationCoroutine("Melee Attack", 0.833f));
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);
        StartCoroutine(StartAnimationCoroutine("Hit",0.667f));
    }
    
}
