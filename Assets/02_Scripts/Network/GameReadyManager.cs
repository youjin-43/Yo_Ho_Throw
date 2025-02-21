using UnityEngine;
using Photon.Pun; // Pun : 포톤 유니티 네트워크의 약자
using Photon.Realtime; // 실시간 통신? 을 위해서

public class GameReadyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab; // 인스펙터에서 할당 
    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // 네트워크를 통해 플레이어 생성 (모든 클라이언트에게 공유됨)
            PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);
        }
    }

    // 랜덤한 위치에서 스폰하도록 설정
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 10f, z);
    }

    // 새로운 플레이어가 들어오면 OnPlayerEnteredRoom()이 호출됨
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 플레이어가 방에 들어옴! - GameReadyManager");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 씬 이동 후, 다시 한번 플레이어 프리팹을 생성
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }


    private void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("spawned") == false)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);

            // 중복 생성을 방지하기 위해, 로컬 플레이어에 "spawned" 값을 저장
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "spawned", true } });
        }
    }
}
