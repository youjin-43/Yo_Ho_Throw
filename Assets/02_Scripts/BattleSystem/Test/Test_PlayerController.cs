using Photon.Pun;
using UnityEngine;

public class Test_PlayerController : MonoBehaviourPun
{
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] Transform firePosition;

    [SerializeField] float speed = 5f;

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 캐릭터 이동 
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
            transform.position += move;

            //총알 발사 
            if (Input.GetKeyDown(KeyCode.F))
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        PhotonNetwork.Instantiate(bulletPrefab.name, firePosition.position, Quaternion.LookRotation((firePosition.position - transform.position).normalized));
    }
}
