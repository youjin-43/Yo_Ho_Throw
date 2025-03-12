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

    private int chestCount=5;
    private List<GameObject> treasureChests = new List<GameObject>(); // 현재 스폰된 보물상자 리스트

    [SerializeField] List<Vector3> usedPosition = new List<Vector3>(); // 이미 스폰된 위치를 저장하는 리스트


    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 스폰 가능
        {
            SpawnTreasureChest();
            StartCoroutine(ChangeTreasureChestPosition());
        }
        // 이벤트 수신 등록
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        // 이벤트 수신 해제
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0) // 이벤트 코드가 0일 때
        {
            int chestViewID = (int)photonEvent.CustomData;
            DestroyTreasureChest(chestViewID); // 보물상자 삭제
        }
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
                //GameObject chest = Instantiate(chestPrefab, spawnPosition, randomRotation);
                GameObject chest = PhotonNetwork.Instantiate(chestPrefab.name, spawnPosition, randomRotation, 0);
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

    // RPC 메서드 추가
    [PunRPC]
    public void DestroyTreasureChest(int chestViewID)
    {
        PhotonView chestView = PhotonView.Find(chestViewID);
        if (chestView != null)
        {
            Destroy(chestView.gameObject); // 보물상자 삭제
            treasureChests.Remove(chestView.gameObject); // 리스트에서 제거
        }
    }

    public void RequestDestroyChest(int chestViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyTreasureChest(chestViewID); // 마스터 클라이언트가 직접 삭제
        }
        else
        {
            // 마스터 클라이언트에게 보물상자 삭제 요청
            PhotonNetwork.RaiseEvent(0, chestViewID, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
    }

}
