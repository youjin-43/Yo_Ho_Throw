using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
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
    /// </summary>
    /// <param name="animName"></param> 
    /// <param name="frame"></param>
    /// <param name="layerWeight"></param>
    /// <param name="targetLayer"></param>
    /// <returns></returns>
    IEnumerator StartAnimationCoroutine(string animName , float frame , int layerIndex = 0 , float layerWeight = 1 )
    {   
        anim.SetTrigger(animName);  
        anim.SetLayerWeight(layerIndex, layerWeight);
        yield return new WaitForSeconds(frame);
        anim.SetTrigger(animName);
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

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
        { 
            targetPosition = hit.point;
        }
        else
        {
            //힛 안됐을 때 카메라앞 10미터를 바라봄
            targetPosition = camTransform.position + camTransform.forward * 10f;
        }

        
        targetAim = targetPosition;
        targetAim.y = transform.position.y;
        aimDir = (targetAim - transform.position).normalized;

       
        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 30f);
    }

    public void Dash()
    {
        StartCoroutine(StartAnimationCoroutine("Dash", 0.333f , 1, 0f));
        
    } 
}
