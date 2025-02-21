using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("PlayerNameInput")]
    public GameObject PlayerNameInputArea; // 플레이어 이름을 받는 UI 영역 
    public TMP_InputField playerNameInput; // 플레이어 이름 입력 필드
    public Button connectButton; // 연결 버튼

    [Header("RoomList")]
    public GameObject roomPanel; // 방 목록을 표시하는 패널
    public GameObject roomListItemPrefab; // 방 목록 아이템 프리팹
    public Transform roomListContent; // 방 목록이 추가될 부모 오브젝트
    public Button createNewRoomButton; // 새로운 방 생성하는 버튼

    enum roomListItemPrefabChilds
    {
        RoomName,
        PlayerCount,
        JoinButton,
        LockIcon
    }

    [Header("CreatNewRoom")]
    public GameObject CreatNewRoomArea; // 새로운 방을 생셩하는 UI 영역 
    public TMP_InputField roomNameInput; // 방 이름 입력 필드
    public Toggle passwordToggle; // 비밀번호 사용 여부 체크박스
    public GameObject SetPasswordArea;
    public TMP_InputField roomPasswordInput; // 방 비밀번호 입력 필드
    public TMP_Dropdown maxPlayersDropdown; // 최대 플레이어 수 선택 드롭다운
    public TMP_Dropdown modeDropdown; // 게임 모드 선택 드롭다운 (개인전/팀전)
    public Button createRoomButton; // 방 생성 버튼

    [Header("JoinRoom")]
    private string selectedRoomName = ""; // 선택된 방 이름 저장
    public GameObject passwordPromptPanel; // 비밀번호 입력 패널
    public TMP_InputField passwordInput; // 비밀번호 입력 필드
    public Button passwordSubmitButton; // 비밀번호 확인 버튼
    private string roomPassword = ""; // 선택된 방의 비밀번호 저장 변수

    void Start()
    {
        // PlayerNameInput
        PlayerNameInputArea.SetActive(true);
        connectButton.onClick.AddListener(ConnectToPhoton); // 포톤에 연결

        // RoomList
        roomPanel.SetActive(false);
        createNewRoomButton.onClick.AddListener(ShowCreatRoomUI);

        // CreatNewRoom
        CreatNewRoomArea.SetActive(false);
        SetPasswordArea.gameObject.SetActive(false); // 비밀번호 설정 영역 - 기본 : 비활성화
        passwordToggle.onValueChanged.AddListener(TogglePasswordInput);
        createRoomButton.onClick.AddListener(CreateRoom);

        // JoinRoom
        passwordPromptPanel.SetActive(false);
        passwordSubmitButton.onClick.AddListener(JoinRoomWithPassword);
    }

    #region PlayerNameInput + RoomList
    void ConnectToPhoton()
    {
        //지역 kr로 고정.이 부분이 없으면 자동으로 지역을 찾는데,다른 지역에 걸릴 경우 네트워크를 통해 만날 수 없다.
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";

        PhotonNetwork.AutomaticallySyncScene = true; // 이렇게 설정하면 방장이 이동한 씬(GameReadyScene)으로 새로운 플레이어도 자동 이동됨!
        // 이 설정이 꺼져 있으면, 새로 들어온 플레이어는 방장이 이동한 씬으로 자동 이동되지 않음! 이 경우, 새로 들어온 플레이어도 수동으로 씬을 이동시켜야 함 

        // 포톤 서버와 통신 횟수 확인. 초당 30회. 30이 떠야 정상
        Debug.Log("포톤 서버와 통신 횟수 확인 : " + PhotonNetwork.SendRate);

        // 플레이어 이름을 Photon 네트워크 닉네임으로 설정
        PhotonNetwork.NickName = playerNameInput.text;
        GameManager.Instance.UserId = playerNameInput.text;

        // 포톤 서버에 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버에 연결되면 로비에 참가
        Debug.Log("로비 입장 완료! 방 목록 업데이트 대기 중...");
        PhotonNetwork.JoinLobby(); // 로비에 참가하면 방 목록이 갱신됨
    }

    public override void OnJoinedLobby()
    {
        // 로비에 입장하면 방 목록 UI 활성화
        PlayerNameInputArea.SetActive(false);
        roomPanel.SetActive(true);
    }

    /* OnRoomListUpdate(List<RoomInfo> roomList) 함수는 포톤(PUN)의 로비에서 방 목록이 갱신될 때 자동으로 호출

  언제 실행되는가?
  1.처음 로비에 입장할 때 (PhotonNetwork.JoinLobby() 실행 후)
  2.새로운 방이 생성되었을 때
  3.기존 방이 삭제되었을 때 (방장이 방을 나가거나, 모든 플레이어가 퇴장했을 때)
  4.방 속성이 변경되었을 때 (최대 인원 변경, 비밀번호 추가/제거 등)
  5. 다른 플레이어가 방을 만들거나 나가면 → OnRoomListUpdate() 자동 실행
  */
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("방 목록이 업데이트됨!");

        // 기존 목록 제거
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 방 목록 표시
        foreach (RoomInfo room in roomList) // roomList 매개변수에는 현재 로비에 존재하는 모든 방 정보가 담겨 있음
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.RoomName).GetComponent<TMP_Text>().text = room.Name;
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.PlayerCount).GetComponent<TMP_Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}";
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.JoinButton).GetComponent<Button>().onClick.AddListener(() => TryJoinRoom(room)); 
        }
    }

    #endregion

    public void ShowCreatRoomUI()
    {
        roomPanel.SetActive(false);
        CreatNewRoomArea.SetActive(true);
    }

    #region CreatNewRoom

    public void CreateRoom()
    {
        // 방 옵션 설정
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)(maxPlayersDropdown.value + 2); // 최소 2명부터
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"password", roomPasswordInput.text}, // 방 비밀번호 설정
            {"mode", modeDropdown.options[modeDropdown.value].text} // 게임 모드 설정
        };
        options.CustomRoomPropertiesForLobby = new string[] { "mode" }; // 로비에서 표시할 커스텀 속성
        
        // 방 생성 요청
        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }

    void TogglePasswordInput(bool isOn)
    {
        roomPasswordInput.interactable = isOn;
        SetPasswordArea.gameObject.SetActive(isOn);
    }

    #endregion

    #region JoinRoom

    public void TryJoinRoom(RoomInfo room)
    {
        // 방의 비밀번호 가져오기
        roomPassword = (string)room.CustomProperties["password"];
        selectedRoomName = room.Name; // 선택한 방 이름 저장
        if (!string.IsNullOrEmpty(roomPassword))
        {
            // 비밀번호 입력창 활성화
            passwordPromptPanel.SetActive(true);
        }
        else
        {
            // 비밀번호가 없는 경우 바로 입장
            PhotonNetwork.JoinRoom(selectedRoomName);

            /*
            일반 클라이언트의 흐름
            1. 마스터 클라이언트가 씬을 변경했기 때문에 JoinRoom()을 하게 되면 자동으로 GameReadyScene으로 이동
            2. GameReadyManager.OnJoinedRoom() 실행
            3. SpawnPlayer() 실행 
             */
        }
    }

    public void JoinRoomWithPassword()
    {
        // 입력한 비밀번호가 맞는 경우 방 입장
        if (passwordInput.text == roomPassword)
        {
            PhotonNetwork.JoinRoom(selectedRoomName); 
            passwordPromptPanel.SetActive(false);
        }
        else
        {
            Debug.Log("비밀번호가 틀렸습니다!");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공!");

        //내가 방장인지 확인
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("내가 방장이다!!!");
            PhotonNetwork.LoadLevel("GameReadyScene");
            // 전에 PhotonNetwork.AutomaticallySyncScene를 true로 설정했기 때문에 방장이 이동한 씬(GameReadyScene)으로 새로운 플레이어도 자동 이동됨!

        }
        else
        {
            Debug.Log("나는 클라이언트다!!");
        }

        Debug.Log($"룸 입장 여부 = {PhotonNetwork.InRoom}");
        Debug.Log($"현재 룸의 인원수 = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players) Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}"); //ActorNumber:몇번째로 들어왔냐
    }
    #endregion
}
