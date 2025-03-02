using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using System;

public enum RoomProperties
{
    mode,
    password,
    teamCount
}

/// <summary>
/// 포톤 매니저로 대체됨
/// </summary>
public class LobbyManager_Lagacy : MonoBehaviourPunCallbacks
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
        GameMode,
        JoinButton,
        LockIcon
    }

    [Header("CreatNewRoom")]
    public GameObject CreatNewRoomArea; // 새로운 방을 생셩하는 UI 영역 
    public TMP_InputField roomNameInput; // 방 이름 입력 필드

    // 비밀번호 설정 
    public Toggle passwordToggle; // 비밀번호 사용 여부 체크박스
    public GameObject SetPasswordArea;
    public TMP_InputField roomPasswordInput; // 방 비밀번호 입력 필드

    // 모드 선택
    public TMP_Dropdown modeDropdown; // 게임 모드 선택 드롭다운 (개인전/팀전)

    //public TMP_Dropdown MaxPlayerCountDropdown_DeathMatch;
    public TMP_Text MaxPlayerCount; // 인원 수
    public Button decreaseButton;
    public Button increaseButton;

    private int currentPlayerCount = 2; // 기본 인원 수


    public TMP_Dropdown MaxPlayerCountDropdown_TeamMatch; 
    public TMP_Dropdown teamCountDropdown; // 팀 개수 선택 드롭다운
    public GameObject TeamCountArea; // 팀 개수 설정 UI 영역

    public Button createRoomButton; // 방 생성 버튼
    public Button createRoomCancelButton; // 방 생성 취소 버튼

    [Header("JoinRoom")]
    private string selectedRoomName = ""; // 선택된 방 이름 저장
    private string roomPassword = ""; // 선택된 방의 비밀번호 저장 변수
    public GameObject passwordPromptPanel; // 비밀번호 입력 패널
    public TMP_InputField passwordInput; // 비밀번호 입력 필드
    public Button passwordSubmitButton; // 비밀번호 확인 버튼
    
    void Start()
    {
        #region PlayerNameInput
        PlayerNameInputArea.SetActive(true);
        connectButton.onClick.AddListener(ConnectToPhoton); // 포톤에 연결
        #endregion

        #region RoomList
        roomPanel.SetActive(false);
        createNewRoomButton.onClick.AddListener(ShowCreatRoomUI);
        #endregion

        #region CreatNewRoom
        CreatNewRoomArea.SetActive(false);

        // 비밀번호 설정 영역 - 기본 : 비활성화
        SetPasswordArea.gameObject.SetActive(false); 
        passwordToggle.onValueChanged.AddListener(TogglePasswordInput);

        // 모드
        TeamCountArea.SetActive(false);
        modeDropdown.onValueChanged.AddListener(UpdateMaxPlayerSelectUI);
        MaxPlayerCountDropdown_TeamMatch.onValueChanged.AddListener(UpdateMaxTeamCount);

        // 버튼
        decreaseButton.onClick.AddListener(DecreasePlayerCount);
        increaseButton.onClick.AddListener(IncreasePlayerCount);
        createRoomButton.onClick.AddListener(CreateRoom);
        createRoomCancelButton.onClick.AddListener(CreateRoomCancel);
        #endregion

        #region JoinRoom
        passwordPromptPanel.SetActive(false);
        passwordSubmitButton.onClick.AddListener(JoinRoomWithPassword);
        #endregion
    }

    #region PlayerNameInput + RoomList
    void ConnectToPhoton()
    {
        // 이름 입력란이 비었는지 화인 
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogWarning("이름을 입력해야 합니다!");
            return; // 연결 중단
        }

        //지역 kr로 고정.이 부분이 없으면 자동으로 지역을 찾는데,다른 지역에 걸릴 경우 네트워크를 통해 만날 수 없다.
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";

        PhotonNetwork.AutomaticallySyncScene = true; // 이렇게 설정하면 방장이 이동한 씬(GameReadyScene)으로 새로운 플레이어도 자동 이동됨!
        // 이 설정이 꺼져 있으면, 새로 들어온 플레이어는 방장이 이동한 씬으로 자동 이동되지 않음! 이 경우, 새로 들어온 플레이어도 수동으로 씬을 이동시켜야 함 

        // 포톤 서버와 통신 횟수 확인. 초당 30회. 30이 떠야 정상
        Debug.Log("포톤 서버와 통신 횟수 확인 : " + PhotonNetwork.SendRate);

        // 플레이어 이름을 Photon 네트워크 닉네임으로 설정
        PhotonNetwork.NickName = playerNameInput.text;
        //GameManager.Instance.UserId = playerNameInput.text;

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
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.RoomName).GetComponent<TMP_Text>().text = room.Name; // 룸 이름
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.PlayerCount).GetComponent<TMP_Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}"; // 최대 플레이어 수 

            // 모드 정보 표시 
            if (room.CustomProperties.ContainsKey(RoomProperties.mode.ToString()))
            {
                string mode = (string)room.CustomProperties[RoomProperties.mode.ToString()];
                Debug.Log($"방 목록 업데이트 - 모드: {mode}");
                roomItem.transform.GetChild((int)roomListItemPrefabChilds.GameMode).GetComponent<TMP_Text>().text = mode;
            }
            else
            {
                Debug.LogWarning($"방 목록 업데이트 - 모드 정보 없음 (룸 이름: {room.Name})");
            }

            // 방 입장 버튼 설정
            roomItem.transform.GetChild((int)roomListItemPrefabChilds.JoinButton).GetComponent<Button>().onClick.AddListener(() => TryJoinRoom(room));

            // 비밀번호가 있는 경우 🔒 아이콘 활성화
            if (room.CustomProperties.ContainsKey("password")) roomItem.transform.GetChild((int)roomListItemPrefabChilds.LockIcon).gameObject.SetActive(true);
        }
    }

    #endregion

    #region CreatNewRoom

    public void ShowCreatRoomUI()
    {
        roomPanel.SetActive(false);
        CreatNewRoomArea.SetActive(true);
        UpdatePlayerCountDisplay(); // 초기 인원 수 표시
    }
    private void DecreasePlayerCount()
    {
        if (currentPlayerCount > 2) // 최소 인원 수 제한
        {
            currentPlayerCount--;
            UpdatePlayerCountDisplay();
        }
    }

    private void IncreasePlayerCount()
    {
        if (currentPlayerCount < 8) // 최대 인원 수 제한
        {
            currentPlayerCount++;
            UpdatePlayerCountDisplay();
        }
    }

    private void UpdatePlayerCountDisplay()
    {
        MaxPlayerCount.text = currentPlayerCount.ToString();
    }


    public void CreateRoom()
    {
        // 방 옵션 설정
        RoomOptions options = new RoomOptions();
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        // 비밀번호가 설정된 경우 체크
        if (passwordToggle.isOn)
        {
            if (string.IsNullOrEmpty(roomPasswordInput.text))
            {
                Debug.LogWarning("비밀번호를 입력해야 합니다!");
                return; // 방 생성 중단
            }

            options.CustomRoomProperties.Add(RoomProperties.password.ToString(), roomPasswordInput.text);
        }

        // 게임 모드 설정
        string selectedMode = modeDropdown.options[modeDropdown.value].text;
        Debug.Log($"방 생성 - 모드: {selectedMode}");
        options.CustomRoomProperties.Add(RoomProperties.mode.ToString(), selectedMode);

        // 최대 인원 수 설정 
        options.MaxPlayers = (byte)currentPlayerCount;
        /*if (selectedMode == GameMode.TeamMatch.ToString())
        {
            options.MaxPlayers = int.Parse(MaxPlayerCountDropdown_TeamMatch.options[MaxPlayerCountDropdown_TeamMatch.value].text); // 최대 플레이어 설정 
            int selectedTeamCount = int.Parse(teamCountDropdown.options[teamCountDropdown.value].text); // 팀 모드일 경우 팀 개수 설정
            options.CustomRoomProperties.Add(RoomProperties.teamCount.ToString(), selectedTeamCount);
        }
        else
        {
            options.MaxPlayers = MaxPlayerCountDropdown_DeathMatch.value + 2; 
        }*/

        // 로비에서 표시할 커스텀 속성 설정
        options.CustomRoomPropertiesForLobby = new string[] {
            RoomProperties.mode.ToString(),
            RoomProperties.password.ToString(),
            RoomProperties.teamCount.ToString() }
        ; //로비에서 방 목록을 업데이트할 때 모드와 비밀번호 정보를 포함하게 됨 
        // TODO : 유진 - teamCount도 룸 리스트 정보에 포함 시켜야 할까? 

        // 방 생성 요청
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            Debug.LogWarning("룸 이름을 를 입력해야 합니다!");
            return; // 방 생성 중단
        }

        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }

    public void CreateRoomCancel()
    {
        CreatNewRoomArea.SetActive(false);
        roomPanel.SetActive(true);
    }

    void TogglePasswordInput(bool isOn)
    {
        roomPasswordInput.interactable = isOn;
        SetPasswordArea.gameObject.SetActive(isOn);
    }

    /// <summary>
    /// 팀 모드 선택 시 팀 개수 UI 활성화
    /// </summary>
    void UpdateMaxPlayerSelectUI(int modeIndex)
    {
        string selectedMode = modeDropdown.options[modeIndex].text;

        //if (selectedMode == GameMode.TeamMatch.ToString())
        //{
        //    TeamCountArea.SetActive(true);
        //    //MaxPlayerCountDropdown_DeathMatch.gameObject.SetActive(false);
        //    MaxPlayerCountDropdown_TeamMatch.gameObject.SetActive(true);
        //    UpdateMaxTeamCount(MaxPlayerCountDropdown_TeamMatch.value); // 팀 개수 업데이트
        //}
        //else
        //{
        //    TeamCountArea.SetActive(false);
        //    //MaxPlayerCountDropdown_DeathMatch.gameObject.SetActive(true);
        //    MaxPlayerCountDropdown_TeamMatch.gameObject.SetActive(false);
        //}
    }

    // 최대 플레이어 수 변경 시 팀 개수 드롭다운 업데이트
    void UpdateMaxTeamCount(int maxPlayersIndex)
    {
        int maxPlayers = 4 + (int)Math.Pow(2,maxPlayersIndex); // 최소 4부터 시작
        int maxTeams = maxPlayers / 2; // 최대 팀 개수는 (최대 플레이어 수 / 2)

        // 팀 개수 드롭다운 옵션 업데이트
        List<string> teamOptions = new List<string>();
        for (int i = 2; i <= maxTeams; i++) // 최소 2팀부터
        {
            teamOptions.Add(i.ToString());
        }

        teamCountDropdown.ClearOptions();
        teamCountDropdown.AddOptions(teamOptions);
        teamCountDropdown.value = 0; // 기본값: 최소 팀 개수
    }

    #endregion

    #region JoinRoom

    public void TryJoinRoom(RoomInfo room)
    {
        selectedRoomName = room.Name; // 선택한 방 이름 저장

        if (room.CustomProperties.ContainsKey(RoomProperties.password.ToString()))
        {
            Debug.Log("비밀번호가 설정된 방입니다");
            roomPassword = (string)room.CustomProperties[RoomProperties.password.ToString()];

            // 비밀번호 입력 UI 활성화
            passwordPromptPanel.SetActive(true);
        }
        else
        {
            Debug.Log("비밀번호가 설정되지 않은 방입니다. 바로 입장을 시도합니다");

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
