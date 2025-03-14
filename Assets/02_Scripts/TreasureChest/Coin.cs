using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            player.AddCoin(1); // 플레이어의 코인 수 증가
            Destroy(gameObject); // 코인 삭제
        }
    }
}
