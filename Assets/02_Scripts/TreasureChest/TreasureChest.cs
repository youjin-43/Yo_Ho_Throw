using Photon.Pun;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] TreasureManager treasureManager;

    private int attackCount = 0;
    private int minCoin = 1;
    private int maxCoin = 3;
    private bool isOpen = false;

    Animator chestAnimator;

    private PhotonView photonView;

    private void Start()
    {
        chestAnimator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        treasureManager = FindAnyObjectByType<TreasureManager>();
    }

    [PunRPC]
    public void Attack()
    {
        if (!isOpen)
        {
            attackCount++;
            if (attackCount >= 3) photonView.RPC("OpenChest", RpcTarget.All); ;
        }
    }

    [PunRPC]
    private void OpenChest()
    {
        chestAnimator.SetTrigger("Open"); // 보물상자 여는 애니메이션 실행
        isOpen = true;
        int coinCount = Random.Range(minCoin, maxCoin + 1);

        // RPC 호출로 코인 생성
        photonView.RPC("SpawnCoins", RpcTarget.All, coinCount, transform.position);


        treasureManager.RemovePosition(transform.position); // 위치를 다시 스폰 가능하게
        // 보물상자 삭제
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject); // 마스터 클라이언트일때, 보물상자 삭제
        }
        else
        {
            // 다른 플레이어일 때, 마스터 클라이언트에게 보물상자 삭제 요청
            treasureManager.RequestDestroyChest(photonView.ViewID);
        }
    }

    [PunRPC]
    private void SpawnCoins(int coinCount, Vector3 chestPosition)
    {
        float radius = 1.0f; // 원하는 반지름
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < coinCount; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * radius; // 랜덤한 점을 생성
                Vector3 randomPosition = new Vector3(chestPosition.x + randomPoint.x, chestPosition.y + 0.5f, chestPosition.z + randomPoint.y);

                PhotonNetwork.Instantiate(coinPrefab.name, randomPosition, Quaternion.identity, 0); // 코인 생성
            }
        }
    }
}
