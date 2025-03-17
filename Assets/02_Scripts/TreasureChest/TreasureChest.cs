using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TreasureChest : MonoBehaviourPun
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] TreasureManager treasureManager;

    private int attackCount = 0;
    private int minCoin = 1;
    private int maxCoin = 3;
    private bool isOpen = false;

    Animator chestAnimator;

    private void Start()
    {
        chestAnimator = GetComponent<Animator>();
        treasureManager = FindAnyObjectByType<TreasureManager>();
    }

    [PunRPC]
    public void Attack()
    {
        if (!isOpen)
        {
            chestAnimator.SetTrigger("Hit"); // 보물상자 덜컹이는 애니메이션 실행
            attackCount++;
            if (attackCount >= 3) photonView.RPC("OpenChest", RpcTarget.All);
        }
    }

    [PunRPC]
    private void OpenChest()
    {
        chestAnimator.SetTrigger("Open"); // 보물상자 여는 애니메이션 실행
        isOpen = true;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(OpenChestCoroutine());
        }
    }
    IEnumerator OpenChestCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        int coinCount = Random.Range(minCoin, maxCoin + 1);

        treasureManager.SpawnCoins(coinCount, transform.position);

        treasureManager.RemovePosition(transform.position); // 위치를 다시 스폰 가능하게

        PhotonNetwork.Destroy(gameObject); // 마스터 클라이언트일때, 보물상자 삭제
    }

    //[PunRPC]
    //private void SpawnCoins(int coinCount, Vector3 chestPosition)
    //{
    //    float radius = 1.0f; // 원하는 반지름

    //    for (int i = 0; i < coinCount; i++)
    //    {
    //        Vector2 randomPoint = Random.insideUnitCircle * radius; // 랜덤한 점을 생성
    //        Vector3 randomPosition = new Vector3(chestPosition.x + randomPoint.x, chestPosition.y + 0.5f, chestPosition.z + randomPoint.y);

    //        PhotonNetwork.Instantiate(coinPrefab.name, randomPosition, Quaternion.identity, 0); // 코인 생성
    //    }
    //}
}
