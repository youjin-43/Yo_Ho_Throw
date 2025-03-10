using UnityEngine;
using Photon.Pun; // Pun : 포톤 유니티 네트워크의 약자
using Photon.Realtime; // 실시간 통신? 을 위해서
using Unity.Cinemachine;

public class GameReadyNetworkManager : MonoBehaviourPunCallbacks
{
    GameReadyUIManager gameReadyUIManager;
    [SerializeField] GameObject playerPrefab; // 인스펙터에서 할당

    [SerializeField] Transform mainCamera;
    [SerializeField] CinemachineCamera world_followCam;

    [SerializeField] GameObject player;
    [SerializeField] Transform camaraRoot;

    // 마스터 클라이언트만 실행됨 
    void Start()
    {
        gameReadyUIManager = GetComponent<GameReadyUIManager>();
        // TODO : 유진 -  UI 모든 클라이언트에게 갱신되는지 확인해야함 

        if (PhotonNetwork.InRoom)
        {
            // 네트워크를 통해 플레이어 생성 (모든 클라이언트에게 공유됨)
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("방 입장 성공!");

        //내가 방장인지 확인
        if (PhotonNetwork.IsMasterClient) Debug.Log("내가 방장이다!!!");
        else Debug.Log("나는 클라이언트다!!");

        // 디버그
        Debug.Log($"룸 입장 여부 = {PhotonNetwork.InRoom}");
        Debug.Log($"현재 룸의 인원수 = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players) Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}"); //ActorNumber:몇번째로 들어왔냐

        // 플레이어 프리팹을 생성
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // TODO : 중복 처리가 필요할까? 
        //if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("spawned") == false)
        //{
        //    PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);

        //    // 중복 생성을 방지하기 위해, 로컬 플레이어에 "spawned" 값을 저장
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "spawned", true } });
        //    Debug.Log($"SpawnPlayer - {PhotonNetwork.LocalPlayer.ActorNumber}");
        //}

        player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);
        Debug.Log($"Spawned Player: {player}");

        // 카메라에 루트 셋팅 
        camaraRoot = player.GetComponent<PlayerController>().CinemachineCameraTarget.transform;

        player.GetComponent<PhotonView>().RPC("OnInLobby", RpcTarget.All);

        world_followCam.Target.TrackingTarget = camaraRoot;
        world_followCam.Target.LookAtTarget = camaraRoot;
        world_followCam.Target.CustomLookAtTarget = true;

        // --------------------------------------------------------------------------------------
        //world_aimCam.Follow = camaraRoot;

        // 플레이어에 카메라 셋팅
        //player.GetComponent<PlayerController>().aimCam = world_aimCam;
        //player.GetComponent<PlayerController>().cameraTransform = mainCamera.transform;


    }

    // 랜덤한 위치에서 스폰하도록 설정
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 10f, z);
    }

    public void GameStart()
    {
        //룸 닫고
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        Debug.Log("현재 방 오픈 여부: " + PhotonNetwork.CurrentRoom.IsOpen);
        Debug.Log("게임 시작!");
        PhotonNetwork.LoadLevel("Test_BattleSystem");
    }

    // 새로운 플레이어가 들어오면 OnPlayerEnteredRoom()이 호출됨
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.ActorNumber}.{newPlayer.NickName} 플레이어가 방에 들어옴!");
        gameReadyUIManager.UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.ActorNumber}.{otherPlayer.NickName} 플레이어가 방에서 나감!");
        gameReadyUIManager.UpdatePlayerListUI();
    }
}
