using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [SerializeField] GameObject chestPrefab;
    [SerializeField] Transform[] spawnPoints; // 스폰될 위치 리스트

    private int chestCount=5;
    private List<GameObject> treasureChests = new List<GameObject>(); // 현재 스폰된 보물상자 리스트

    List<Vector3> usedPosition = new List<Vector3>(); // 이미 스폰된 위치를 저장하는 리스트

    void Start()
    {
        SpawnTreasureChest();
        StartCoroutine(ChangeTreasureChestPosition());
    }

    private void SpawnTreasureChest()
    {
        while (treasureChests.Count < chestCount) 
        {
            // 미리 정해둔 위치 중 랜덤으로 한 곳 가져오기
            Vector3 spawnPosition = GetRandomSpawnPoint();

            if (!usedPosition.Contains(spawnPosition)) // 중복 체크
            {
                usedPosition.Add(spawnPosition); // 위치를 사용한 것으로 표시

                // 랜덤한 y축 회전값 설정
                float randomYRotation = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);

                // 보물상자 생성
                GameObject chest = Instantiate(chestPrefab, spawnPosition, randomRotation);
                treasureChests.Add(chest); // 현재 보물상자 리스트에 추가
            }
        }
    }

    private IEnumerator ChangeTreasureChestPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // 1분 대기

            // 모든 보물상자 삭제
            foreach (GameObject chest in treasureChests)
            {
                Destroy(chest);
            }
            treasureChests.Clear(); // 보물상자 리스트 초기화
            usedPosition.Clear(); // 사용된 위치 초기화

            // 새로운 보물상자 스폰
            SpawnTreasureChest();
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        // 미리 정해둔 위치 중 랜덤으로 한 곳 반환
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }

    public void RemovePosition(Vector3 position)
    {
        usedPosition.Remove(position); // 사용된 위치에서 제거. 다시 스폰 가능
    }
}
