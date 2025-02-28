using Photon.Pun;
using UnityEngine;

public class Test_Bullet : MonoBehaviourPun
{
    public float speed = 10f;
    public float destroyTime = 1f;

    private void Start()
    {
        if (photonView.IsMine) // 내 총알만 속도 적용
        {
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;

            Invoke("DestroyBullet", destroyTime); // 1초 뒤에 네트워크에서 삭제
        }

    }
    void DestroyBullet()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView photonView = other.GetComponent<PhotonView>();

        if (photonView == null) return;

        Debug.Log("Bullet Actor : " + this.photonView.OwnerActorNr.ToString() + " / Hit Actor : " + photonView.OwnerActorNr.ToString());

        if (this.photonView.OwnerActorNr != photonView.OwnerActorNr)
        {
            BattleSystem.Instance.RegisterKill(this.photonView.OwnerActorNr, photonView.OwnerActorNr);
        }
    }
}
