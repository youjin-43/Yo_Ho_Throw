using UnityEngine;
using Photon.Pun; // Pun : 포톤 유니티 네트워크의 약자
using Photon.Realtime; // 실시간 통신? 을 위해서
using Unity.Cinemachine;
using ExitGames.Client.Photon; // Photon CustomProperties 사용

public class GameReadyNetworkManager : MonoBehaviourPunCallbacks
{
    GameReadyUIManager gameReadyUIManager;
    [SerializeField] GameObject playerPrefab; // 인스펙터에서 할당

    [SerializeField] Transform mainCamera;
    [SerializeField] CinemachineCamera world_followCam;

    [SerializeField] GameObject player;
    [SerializeField] Transform camaraRoot;

    public static event System.Action OnGameStart;

    // 마스터 클라이언트만 실행됨 
    void Start()
    {
        PhotonManager.Instance.UpdatePlayerSceneProperty();

        if (PhotonNetwork.IsMasterClient) PhotonNetwork.AutomaticallySyncScene = true; // 자동 동기화 활성화 (이후 씬 이동 시 동기화됨)

        gameReadyUIManager = GetComponent<GameReadyUIManager>();

        if (PhotonNetwork.InRoom)
        {
            // 네트워크를 통해 플레이어 생성 (모든 클라이언트에게 공유됨)
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[GameReadyNetworkManager] OnJoinedRoom 실행됨 ");
        // 마스터 클라이언트는 여기 실행이 안됨 
        //OnJoinedRoom()은 방에 처음 입장할 때만 실행되는 콜백 함수 
        //마스터 클라이언트는 PhotonNetwork.LoadLevel을 통해 씬을 이동하지만, 이미 방에 들어와 있는 상태이므로 OnJoinedRoom()이 다시 호출되지 않음 .
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);
        Debug.Log($"{PhotonNetwork.NickName} 플레이어를 스폰했습니다");

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



        // ㅇㅎㅈ
        GameManager.Instance.CachePlayer(player);
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
        OnGameStart?.Invoke(); // 게임 시작 이벤트 발생

        //룸 닫고
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        Debug.Log("현재 방 오픈 여부: " + PhotonNetwork.CurrentRoom.IsOpen);
        Debug.Log("게임 시작!");

        PhotonNetwork.LoadLevel(SceneList.Test_BattleSystem.ToString());
    }

    // 새로운 플레이어가 들어오면 OnPlayerEnteredRoom()이 호출됨
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        gameReadyUIManager.UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        gameReadyUIManager.UpdatePlayerListUI();
    }

    // using ExitGames.Client.Photon; 필요 
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("[GameReadyNetworkManager] OnPlayerPropertiesUpdate 호출됨");

        if (changedProps.ContainsKey("CurrentScene"))
        {
            Debug.Log($"🎯 {targetPlayer.NickName}의 씬 변경: {changedProps["CurrentScene"]}");
            gameReadyUIManager.UpdatePlayerListUI(); // 🔄 UI 업데이트 (씬 변경 감지 시)
        }
    }
}
