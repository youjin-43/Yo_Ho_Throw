using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Coin : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    Debug.Log("Coin OnTriggerEnter - 코인획득");

                    playerController.AddCoin(1); // 코인 추가

                    PhotonNetwork.Destroy(gameObject); // 소유자일 때, 코인 삭제
                }
            }
        }
    }
}
