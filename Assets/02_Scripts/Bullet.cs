using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;
    public float destroyTime = 1f;

    private void Start()
    {
        if (photonView.IsMine) // 내 총알만 속도 적용
        {
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }

        // 1초 뒤에 네트워크에서 삭제
        Invoke("DestroyBullet", destroyTime);
    }

    void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}