using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [SerializeField] GameObject chestPrefab;
    [SerializeField] Transform[] spawnPoints; // 스폰될 위치 리스트

    private int maxChestCount=5;
    private int currChestCount = 0;

    private List<GameObject> treasureChests = new List<GameObject>(); // 현재 스폰된 보물상자 리스트

    [SerializeField] List<Vector3> usedPosition = new List<Vector3>(); // 이미 스폰된 위치를 저장하는 리스트

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 스폰 가능
        {
            SpawnTreasureChest();
            StartCoroutine(ChangeTreasureChestPosition());
        }
    }
    private void OnEnable() => PhotonNetwork.NetworkingClient.EventReceived += OnEvent; // 이벤트 수신 
    private void OnDisable() => PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; // 이벤트 수신 해제

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0) // 이벤트 코드가 0일 때 (보물상자 삭제)
        {
            DestroyTreasureChest(); // 보물상자 삭제
        }
    }

    private void SpawnTreasureChest()
    {
        while (currChestCount < maxChestCount) 
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
                //GameObject chest = Instantiate(chestPrefab, spawnPosition, randomRotation);
                GameObject chest = PhotonNetwork.Instantiate(chestPrefab.name, spawnPosition, randomRotation, 0);

                treasureChests.Add(chest);

                currChestCount++;
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
                if (chest != null)
                {
                    PhotonNetwork.Destroy(chest); // 네트워크에서 보물상자 삭제
                }
            }
            treasureChests.Clear(); // 보물상자 리스트 초기화
            usedPosition.Clear(); // 사용된 위치 초기화
            currChestCount = 0;

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
    public void DestroyTreasureChest()
    {
        currChestCount--;
    }
}
