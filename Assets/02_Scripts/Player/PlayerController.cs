using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class PlayerController : ThirdPersonController
{
    private StarterAssetsInputs input;
    public GameObject bulletPrefab; // 투사체 프리펩
    public Transform bulletSpawnPoint; // 투사체 생성 위치
    public float bulletSpeed = 10f; // 투사체 속도
    public float bulletArc = 5f; // 투사체가 포물선을 그릴 때 사용 하는 값
    public Transform cameraTransform;
    [Header("Aim")]
    [SerializeField]
    private CinemachineVirtualCamera aimCam;
    [SerializeField]
    private LayerMask targetLayer;
    private Animator anim;
    private Vector3 targetPosition = Vector3.zero;

   
    public bool lookCamera= true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        input = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
        //anim.applyRootMotion = false;
    }

    
    // Update is called once per frame
    void Update()
    {
        
        base.Update();
        
       // LookSameCameraDirection();

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
            anim.SetTrigger("Shoot");
            
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            anim.SetTrigger("Dash");
        }

    }
    public void FixedUpdate()
    {
        base.FixedUpdate();
        
    }

    public void LateUpdate()
    {
        
        
            LookSameCameraDirection();

        
    }

    /// <summary>
    /// 투사체를 생성하여 던지는 함수 
    /// 플레이어 애니메이션의 throw attack에서 호출됨
    /// </summary>
    void ThrowProjectile()
    {
        StartCoroutine(StartAnimationCoroutine("Shoot", 0.24f));

        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
           
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

    /// <summary>
    /// 트리거이름과 애니메이션 길이를 넣어 재생. 다른 레이어의 가중치를 없애고 싶을때 3,4번째
    /// 해당 애니메이션이 실행 될 때 카메라를 따라가지 않는다면 5번째 인자에 false
    /// </summary>
    /// <param name="animName"></param> 
    /// <param name="frame"></param>
    /// <param name="layerWeight"></param>
    /// <param name="targetLayer"></param>
    /// <returns></returns>
    IEnumerator StartAnimationCoroutine(string _animName , float _frame , int _layerIndex = 0 , float _layerWeight = 1 ,bool _lookCamera = true)
    {   
        if(_lookCamera == false)
        {
            lookCamera = false;
        }
        anim.SetTrigger(_animName);  
        anim.SetLayerWeight(_layerIndex, _layerWeight);
        yield return new WaitForSeconds(_frame);
        anim.SetTrigger(_animName);
        LayerReset();
        lookCamera = true;

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

        if (lookCamera)
        {
            transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * 30f);

        }
    }

    public void Dash()
    {
        StartCoroutine(StartAnimationCoroutine("Dash", 0.333f, 1, 0f, false));

        lookCamera = false; 

        
        Vector3 moveDirection = new Vector3(input.move.x, 0, input.move.y).normalized;

        // 이동 입력이 없으면 플레이어가 바라보는 방향으로 대시
        if (moveDirection.magnitude == 0)
        {
            moveDirection = transform.forward;
        }
        else
        {
            // 현재 플레이어가 바라보는 방향 기준으로 이동 방향 변환
            moveDirection = transform.TransformDirection(moveDirection);
        }

        moveDirection.y = 0; // 수직 이동 방지
        transform.rotation = Quaternion.LookRotation(moveDirection);  // 대시 방향을 바라보게 설정
        StartCoroutine(DashMovement(moveDirection)); // 대시 이동 실행
    }

    private IEnumerator DashMovement(Vector3 direction)
    {
        float dashDistance = 3f; 
        float dashTime = 0.2f;  
        float elapsedTime = 0f;

        Vector3 velocity = direction * (dashDistance / dashTime);
        
        while (elapsedTime < dashTime)
        {
            _controller.Move(velocity * Time.deltaTime); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(EnableLookCameraAfterDelay(0.2f));
    }
    IEnumerator EnableLookCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lookCamera = true; 
    }
    
}
