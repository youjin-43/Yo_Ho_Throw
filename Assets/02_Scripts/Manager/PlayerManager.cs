using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : PlayerStatController
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
        input = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
        //anim.applyRootMotion = false;
    }

    // Update is called once per frame
    void Update()
    {
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
            anim.SetBool("Shoot", true);
            StartCoroutine(EndShootCoroutine());
        }
    }
    /// <summary>
    /// 투사체를 생성하여 던지는 함수 
    /// 플레이어 애니메이션의 throw attack에서 호출됨
    /// </summary>
    void ThrowProjectile()
    {
        
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

    
    IEnumerator EndShootCoroutine()
    {
        yield return new WaitForSeconds(0.24f);
        anim.SetBool("Shoot", false);
        
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
}
