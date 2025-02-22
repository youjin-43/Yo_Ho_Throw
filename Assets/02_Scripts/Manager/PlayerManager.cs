using Photon.Pun.Demo.Asteroids;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : PlayerStatController
{
    private StarterAssetsInputs input;
    public GameObject bulletPrefab; // 주석체크
    public Transform bulletSpawnPoint; // 
    public float bulletSpeed = 10f; // 
    public float bulletArc = 5f; // 
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

        if (input.aim)
        {
            aimCam.gameObject.SetActive(true);
        }
        else
        {
            aimCam.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F)) // 
        {
            anim.SetBool("Shoot", true);
            StartCoroutine(EndShootCoroutine());
        }
    }

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
            //GameObject projectile = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.useGravity = false;

                Vector3 throwDirection = (targetPosition - bulletSpawnPoint.position).normalized; //Vector3.up * bulletArc;
                Quaternion rotationOffset = Quaternion.Euler(90, 0, 0); // 필요에 따라 조정
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
