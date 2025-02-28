using UnityEngine;

public class ParringTest : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;

    private float lastFireTime = 0f;

    void Update()
    {

        if (Time.time >= lastFireTime + fireRate)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
    }

    private void FireBullet()
    {
        GameObject bullet = PoolManager.Instance.Pop(bulletPrefab);
        bullet.transform.position = firePoint.position;

        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Rigidbody가 있으면 앞으로 발사
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Quaternion rotationOffset = Quaternion.Euler(90, 0, 0);
            bullet.transform.rotation = Quaternion.LookRotation(firePoint.forward) * rotationOffset;
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}
