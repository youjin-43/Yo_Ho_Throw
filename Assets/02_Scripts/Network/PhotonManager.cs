//포톤 네트워크를 이용하기 위해 아래 두개가 필수 
using System;
using System.Collections.Generic;
using Photon.Pun; // Pun : 포톤 유니티 네트워크의 약자
using Photon.Realtime; // 실시간 통신? 을 위해서
using UnityEngine;

public enum PhotonRoomProperties
{
    mode,
    password
}

public enum PhotonPlayerProperties
{
    IsReady
}
/*
           
    일반 클라이언트의 흐름
    1. 마스터 클라이언트가 씬을 변경했기 때문에 JoinRoom()을 하게 되면 자동으로 GameReadyScene으로 이동
    2. GameReadyManager.OnJoinedRoom() 실행
    3. SpawnPlayer() 실행 
            
*/
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager _instance;
    public static PhotonManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindFirstObjectByType<PhotonManager>();
                if (!_instance)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    _instance = obj.AddComponent(typeof(PhotonManager)) as PhotonManager;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private List<RoomInfo> currentRoomList = new List<RoomInfo>(); // 방 목록을 저장하는 리스트

    #region TITLE

    public void ConnectToPhoton()
    {
        //지역 kr로 고정.이 부분이 없으면 자동으로 지역을 찾는데,다른 지역에 걸릴 경우 네트워크를 통해 만날 수 없다.
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";

        PhotonNetwork.AutomaticallySyncScene = true; // 이렇게 설정하면 방장이 이동한 씬(GameReadyScene)으로 새로운 플레이어도 자동 이동됨!
        // 이 설정이 꺼져 있으면, 새로 들어온 플레이어는 방장이 이동한 씬으로 자동 이동되지 않음! 이 경우, 새로 들어온 플레이어도 수동으로 씬을 이동시켜야 함 

        // 포톤 서버와 통신 횟수 확인. 초당 30회. 30이 떠야 정상
        Debug.Log("포톤 서버와 통신 횟수 확인 : " + PhotonNetwork.SendRate);

        // 플레이어 이름을 Photon 네트워크 닉네임으로 설정
        PhotonNetwork.NickName = GameManager.Instance.UserName;
        
        // 포톤 서버에 연결
        PhotonNetwork.ConnectUsingSettings(); // 세팅한걸로 커넥트. PhotonNetwork를 실제 연결하며 서버 접속. 이거 하면 OnConnectedToMaster()가 호출됨
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        print($"{PhotonNetwork.NickName} : 서버 접속 완료");
        Debug.Log($"로비 입장 여부 = {PhotonNetwork.InLobby}"); // 로비 입장했는지 확인, False. 연결과 로비 입장은 다르기 때문에 False 가 정상.
        PhotonNetwork.JoinLobby(); //서버에 입장한 후 바로 로비로 입장 
    }

    // 로비에 입장했을 때 실행할 이벤트
    public static event Action OnLobbyJoined;

    //로비에 접속 후 호출되는 콜백 함수 
    public override void OnJoinedLobby()
    {
        Debug.Log($"로비 입장 여부 = {PhotonNetwork.InLobby}"); // 로비 입장했는지 확인, True
        OnLobbyJoined?.Invoke(); // 이벤트 발생! (구독한 함수들 실행됨)
    }

    // 방 목록이 변경될 때 실행할 이벤트
    public static event Action<List<RoomInfo>> OnRoomListUpdated;

    // 포톤(PUN)의 로비에서 방 목록이 갱신될 때 자동으로 호출
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("방 목록이 업데이트됨!");
        currentRoomList = new List<RoomInfo>(roomList); // 최신 방 목록 저장
        OnRoomListUpdated?.Invoke(roomList);
    }

    // 방 목록을 가져오는 함수 
    public List<RoomInfo> GetCurrentRoomList()
    {
        return currentRoomList;
    }

    // 방 목록을 갱신하는 함수 추가!
    public void RequestRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("현재 로비에 있음. 다시 방 목록 요청을 위해 LeaveLobby 후 JoinLobby 실행!");
            PhotonNetwork.LeaveLobby(); // 기존 로비 나가기 → OnLeftLobby() 실행됨
        }
        else
        {
            Debug.Log("🔄 로비에 없음. JoinLobby() 실행하여 방 목록 새로 요청!");
            PhotonNetwork.JoinLobby(); // 로비에 입장하여 자동으로 방 목록 갱신
        }
    }

    // LeaveLobby()의 콜백 함수 -> 로비에서 나가면 다시 입장하여 방 목록을 새로 요청
    public override void OnLeftLobby()
    {
        Debug.Log("로비를 떠남. 다시 JoinLobby() 실행하여 방 목록 갱신 요청!");
        PhotonNetwork.JoinLobby();
    }

    public static event Action<string> OnRoomCreationFailed;

    // TitleUIManager에서 createRoomButton을 눌렀을때 실행 
    public void CreateRoom(string roomName, RoomOptions options)
    {
        // 최신 방 목록 가져오기
        List<RoomInfo> roomList = GetCurrentRoomList();

        // 같은 이름의 방이 있는지 확인
        foreach (RoomInfo room in roomList)
        {
            if (room.Name == roomName)
            {
                // 이벤트 발생 (UI에 알림 메시지를 보냄)
                OnRoomCreationFailed?.Invoke($"방 생성 실패: 이미 존재하는 방 이름입니다! ({roomName})");
                return; // 방 생성 중단
            }
        }

        // 중복이 없으면 정상적으로 방 생성
        Debug.Log(
            $"방 생성 요청 - 이름: {roomName}, " +
            $"모드 : {options.CustomRoomProperties[PhotonRoomProperties.mode.ToString()]}, " +
            $"최대 인원: {options.MaxPlayers}"
            );
        PhotonNetwork.CreateRoom(roomName, options);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log($"룸 생성 - 이름 : {PhotonNetwork.CurrentRoom.Name}");
    }

    // 방 입장 요청 이벤트!
    public static event Action<RoomInfo> OnPasswordCheckRequested;

    // TitleUIManager는 “이 방에 입장하고 싶어요!” 신호를 보냄 -> PhotonManager는 그 신호를 듣고 TryJoinRoom 실행
    public void TryJoinRoom(RoomInfo room)
    {
        Debug.Log($"[PhotonManager] 방 입장 시도: {room.Name}");

        // 비밀번호가 설정돼있다면 
        if (room.CustomProperties.ContainsKey(PhotonRoomProperties.password.ToString()))
        {
            Debug.Log("비밀번호가 설정된 방입니다");
            OnPasswordCheckRequested?.Invoke(room); // 비밀번호 입력 UI 활성화 이벤트 호출
        }
        else
        {
            Debug.Log("비밀번호가 설정되지 않은 방입니다. 바로 입장을 시도합니다");
            PhotonNetwork.JoinRoom(room.Name);
        }
    }

    // TitleUIManager의 VerifyPassword에서 호출됨 
    public void JoinRoomByName(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        //내가 방장인지 확인
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("내가 방장이다!!!");
            //PhotonNetwork.LoadLevel("GameReadyScene");
            PhotonNetwork.LoadLevel("GameReadyScene_1");
            // AutomaticallySyncScene를 true로 설정했기 때문에, 새로 들어온 클라이언트도 자동으로 방장이 이동한 씬(GameReadyScene)으로 이동
        }
        else
        {
            Debug.Log("나는 클라이언트다!!"); // 여기 스크립트에서는 실행 안돼야 정상 
        }

        Debug.Log($"룸 입장 여부 = {PhotonNetwork.InRoom}");
        Debug.Log($"현재 룸의 인원수 = {PhotonNetwork.CurrentRoom.PlayerCount}"); // 방장 1명으로 떠야 정상 


        //for 문 처럼 Player 마다 실행 
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}"); //ActorNumber:몇번째로 들어왔냐
        }
    }

    #endregion
}