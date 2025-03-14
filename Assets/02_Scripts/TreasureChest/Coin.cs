using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 플레이어와 충돌 시
        {
            Debug.Log("충돌발생");
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddCoin(1); // 코인 추가
                PhotonNetwork.Destroy(gameObject); // 네트워크에서 코인 삭제
            }
        }
    }
}
