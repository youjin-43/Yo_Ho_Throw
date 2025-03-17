using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] TreasureManager treasureManager;
    private PhotonView photonView;

    private void Start()
    {
        treasureManager = FindAnyObjectByType<TreasureManager>();
        photonView = GetComponent<PhotonView>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddCoin(1); // 코인 추가
                Debug.Log("코인획득");

                // 소유권 확인 후 삭제
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(gameObject); // 마스터 클라이언트일 때, 코인 삭제
                }
                else
                {
                    PhotonNetwork.RaiseEvent(1, photonView.ViewID, RaiseEventOptions.Default, SendOptions.SendReliable);
                }
            }
        }
    }
}
