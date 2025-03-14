using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnColliderEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            Debug.Log("충돌발생");
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddCoin(1); // 코인 추가
                Debug.Log("코인획득");
                PhotonNetwork.Destroy(gameObject); // 네트워크에서 코인 삭제
            }
        }
    }
}
