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

    private void Start()
    {
        chestAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (!isOpen)
        {
            attackCount++;
            if (attackCount >= 3) OpenChest();
        }
    }

    private void OpenChest()
    {
        chestAnimator.SetTrigger("Open"); // 보물상자 여는 애니메이션 실행
        isOpen = true;
        int CoinCount = Random.Range(minCoin, maxCoin + 1);

        Vector3 spawnPosition = transform.position; // 현재 보물상자의 위치
        float radius = 1.0f; // 원하는 반지름

        for (int i = 0; i < CoinCount; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius; // 랜덤한 점을 생성

            // 새로운 위치. 보물상자 주변
            Vector3 randomPosition = new Vector3(spawnPosition.x + randomPoint.x, spawnPosition.y, spawnPosition.z + randomPoint.y);

            Instantiate(coinPrefab, randomPosition, Quaternion.identity); // 코인 생성
        }


        treasureManager.RemovePosition(transform.position); // 위치를 다시 스폰 가능하게
        Destroy(gameObject); // 보물상자 삭제
    }
}
