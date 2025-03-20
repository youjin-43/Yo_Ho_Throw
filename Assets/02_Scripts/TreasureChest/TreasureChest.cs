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

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P)) Attack();
    //}

    [PunRPC]
    public void Attack()
    {
        if (!isOpen)
        {
            // 보물상자 덜컹이는 애니메이션 실행
            chestAnimator.SetTrigger("Hit"); 
            AnimateTreasureChest();

            attackCount++;
            if (attackCount >= 3)
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC("OpenChest", RpcTarget.All);

                //OpenChest();
            }
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
        int coinCount = Random.Range(minCoin, maxCoin + 1);
        treasureManager.SpawnCoins(coinCount, transform.position);

        yield return new WaitForSeconds(0.48f);

        treasureManager.RemovePosition(transform.position); // 위치를 다시 스폰 가능하게

        PhotonNetwork.Destroy(gameObject); // 마스터 클라이언트일때, 보물상자 삭제
    }

    private void AnimateTreasureChest()
    {
        Quaternion originalRotation = gameObject.transform.rotation;
        Quaternion[] rotations = new Quaternion[]
        {
        originalRotation * Quaternion.Euler(-4,0,-4), 
        originalRotation * Quaternion.Euler(4,0,4),
        originalRotation,
        originalRotation * Quaternion.Euler(-2,0,2),
        originalRotation * Quaternion.Euler(2,0,-2),
        originalRotation // 원래 회전
        };

        StartCoroutine(RotateChest(rotations));
    }

    private IEnumerator RotateChest(Quaternion[] rotations)
    {
        float duration = 0.05f; // 회전 위치 변경 간격
        for (int i = 0; i < rotations.Length; i++)
        {
            float elapsedTime = 0f;
            Quaternion startingRot = gameObject.transform.rotation;

            while (elapsedTime < duration) // 움직임 부드럽게
            {
                gameObject.transform.rotation = Quaternion.Lerp(startingRot, rotations[i], (elapsedTime / duration)); 
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            gameObject.transform.rotation = rotations[i]; // 설정한 시간에 정확한 회전값이 되도록
        }
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
