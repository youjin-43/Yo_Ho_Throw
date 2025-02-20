using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private StarterAssetsInputs input;
    public GameObject bulletPrefab; // 던질 프리팹
    public Transform bulletSpawnPoint; // 프리팹이 생성될 위치
    public float bulletSpeed = 10f; // 프리팹 초기 속도
    public float bulletArc = 5f; // 포물선 곡률 조정
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
    }

    // Update is called once per frame
    void Update()
    {
        LookSameCameraDirection();

        if (input.aim)
        {
            aimCam.gameObject.SetActive(true);
        }
        else
        {
            aimCam.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F)) // 발사 입력 감지 (Fire1 버튼)
        {
            anim.SetBool("Shoot", true);
            StartCoroutine(EndShootCoroutine());
        }
    }

    void ThrowProjectile()
    {
        
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            //TODO : 풀매니저 
            GameObject projectile = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.useGravity = false;
                Vector3 throwDirection = (targetPosition - bulletSpawnPoint.position).normalized; //Vector3.up * bulletArc; 포물선일때
                rb.linearVelocity = throwDirection*bulletSpeed;
            }
        }
    }

    public void EndShoot()
    {
        anim.SetBool("Shoot", false);
    }
    IEnumerator EndShootCoroutine()
    {
        yield return new WaitForSeconds(0.24f);
        anim.SetBool("Shoot", false);
        
    }
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
            
            targetPosition = camTransform.position + camTransform.forward * 10f;
        }

        
        targetAim = targetPosition;
        targetAim.y = transform.position.y;
        aimDir = (targetAim - transform.position).normalized;

       
        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 30f);
    }
}
