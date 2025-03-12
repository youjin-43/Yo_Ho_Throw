using System;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    // 인스펙터에서 할당

    [Header("Notice")]
    public GameObject NoticePannel; // 알림 패널 오브젝트 
    public TMP_Text NoticeText; // 알림 텍스트 

    [Header("PlayerNameInput")]
    public GameObject PlayerNameInputArea; // 플레이어 이름을 받는 UI 영역 
    public TMP_InputField playerNameInput; // 플레이어 이름 입력 필드
    public Button connectButton; // 연결 버튼

    [Header("RoomList")]
    public GameObject RoomListArea; // 방 목록을 표시하는 패널
    public GameObject roomListItemPrefab; // 방 목록 아이템 프리팹
    public Transform roomListContent; // 방 목록이 추가될 부모 오브젝트
    public Button createNewRoomButton; // 새로운 방 생성하는 버튼
    public Button ReloadingButton;
    enum roomItemChilds
    {
        RoomName,
        //GameMode,
        PlayerCount,
        LockIcon,
        JoinButton
    }

    [Header("CreatNewRoom")]
    public GameObject CreatNewRoomArea; // 새로운 방을 생셩하는 UI 영역 
    public TMP_InputField roomNameInput; // 방 이름 입력 필드

    // 비밀번호 설정 
    public Toggle passwordToggle; // 비밀번호 사용 여부 체크박스
    public GameObject SetPasswordArea;
    public TMP_InputField roomPasswordInput; // 방 비밀번호 입력 필드

    // 모드 선택
    //public TMP_Dropdown modeDropdown; // 게임 모드 선택 드롭

    // 인원 수
    public TMP_Text MaxPlayerCount;
    private int currentPlayerCount = 2; // 기본 인원 수
    public Button decMaxPlayersButton;
    public Button incMaxPlayersButton;

    public Button createRoomButton; // 방 생성 버튼
    public Button createRoomCancelButton; // 방 생성 취소 버튼

    [Header("JoinRoom")]
    private string selectedRoomName = ""; // 선택된 방 이름 저장
    private string roomPassword = ""; // 선택된 방의 비밀번호 저장 변수
    public GameObject passwordPromptPanel; // 비밀번호 입력 패널
    public TMP_InputField passwordInput; // 비밀번호 입력 필드
    public Button passwordSubmitButton; // 비밀번호 확인 버튼
    public Button passwordCancelButton; // 비밀번호 확인 버튼

    private void Start()
    {
        #region Subscribe PhotonManagerEvent
        PhotonManager.OnLobbyJoined += ShowRoomListPanel;
        PhotonManager.OnRoomListUpdated += UpdateRoomList;
        PhotonManager.OnPasswordCheckRequested += ShowPasswordPannel;
        PhotonManager.OnRoomCreationFailed += ShowNoticePannel; // 방 생성 실패 이벤트 구독

        #endregion

        NoticePannel.SetActive(false);

        #region PlayerNameInput
        PlayerNameInputArea.SetActive(false);
        connectButton.onClick.AddListener(() =>
        {
            if (Check_playerNameInput_IsEmpty()) // 이름이 입력되었는지 확인
            {
                SetUserName(); // 게임 매니저에 유저 네임 할당 
                PhotonManager.Instance.ConnectToPhoton(); 
            }
        });
        #endregion

        #region RoomList
        RoomListArea.SetActive(false);
        createNewRoomButton.onClick.AddListener(ShowCreatRoomUI);
        ReloadingButton.onClick.AddListener(() =>
        {
            Debug.Log("새로고침 버튼 클릭됨! 최신 방 목록 요청!");
            PhotonManager.Instance.RequestRoomList(); // 최신 방 목록 요청
        });
        #endregion

        #region CreatNewRoom
        CreatNewRoomArea.SetActive(false);

        // 비밀번호 설정 영역 - 기본 : 비활성화
        SetPasswordArea.gameObject.SetActive(false);
        passwordToggle.onValueChanged.AddListener(TogglePasswordInput);

        // 모드 옵션 셋업 
        //SetupGameModeOptions();

        // Max Player 증감버튼
        decMaxPlayersButton.onClick.AddListener(DecreasePlayerCount);
        incMaxPlayersButton.onClick.AddListener(IncreasePlayerCount);

        // 룸 생성 및 취소 
        createRoomButton.onClick.AddListener(CreateRoom);
        createRoomCancelButton.onClick.AddListener(CreateRoomCancel);
        #endregion

        #region JoinRoom
        passwordPromptPanel.SetActive(false);
        passwordSubmitButton.onClick.AddListener(VerifyPassword);
        passwordCancelButton.onClick.AddListener(CancelVerifyPassword);
        #endregion
    }

    #region PlayerNameInput + RoomList
    /// <summary>
    /// 이름 입력란이 비었는지 화인 
    /// </summary>
    bool Check_playerNameInput_IsEmpty()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            ShowNoticePannel("이름을 입력해야 합니다!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 게임 매니저에 유저 네임 할당 
    /// </summary>
    void SetUserName()
    {
        GameManager.Instance.UserName = playerNameInput.text;
        Debug.Log($"");
    }

    // 로비에 입장하면 실행될 함수
    private void ShowRoomListPanel()
    {
        PlayerNameInputArea.SetActive(false);
        RoomListArea.SetActive(true);
    }

    // 방 목록이 변경될 때 실행할 함수 
    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        Debug.Log($"UI에서 방 목록 업데이트 실행! - 생성돼있는 룸 갯수 : {roomList.Count}");

        // 기존 방 목록 삭제
        foreach (Transform child in roomListContent) Destroy(child.gameObject);

        // 새로운 방 목록 표시
        foreach (RoomInfo room in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);

            // 방 이름 설정
            roomItem.transform.GetChild((int)roomItemChilds.RoomName).GetComponent<TMP_Text>().text = room.Name;

            // 인원 설정 
            roomItem.transform.GetChild((int)roomItemChilds.PlayerCount).GetComponent<TMP_Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}";

            // 방 모드 설정
            //if (room.CustomProperties.ContainsKey(PhotonRoomProperties.mode.ToString()))
            //{
            //    string mode = (string)room.CustomProperties[PhotonRoomProperties.mode.ToString()];
            //    roomItem.transform.GetChild((int)roomItemChilds.GameMode).GetComponent<TMP_Text>().text = mode;
            //}

            // 비밀번호가 설정된 방인지 확인
            GameObject lockIcon = roomItem.transform.GetChild((int)roomItemChilds.LockIcon).gameObject;
            if (room.CustomProperties.ContainsKey(PhotonRoomProperties.password.ToString()))
            {
                lockIcon.SetActive(true); // 🔒 아이콘 활성화
            }
            else
            {
                lockIcon.SetActive(false); // 🔒 아이콘 비활성화
            }

            // 방 입장 버튼 설정
            roomItem.transform.GetChild((int)roomItemChilds.JoinButton).GetComponent<Button>().onClick.AddListener(() => PhotonManager.Instance.TryJoinRoom(room));
        }
    }

    #endregion

    #region CreatNewRoom
    /// <summary>
    /// GameMode enum을 기반으로 modeDropdown을 자동으로 채운다.
    /// </summary>
    //private void SetupGameModeOptions()
    //{
    //    modeDropdown.ClearOptions(); // 기존 옵션 제거
    //    List<string> modeNames = new List<string>();

    //    foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
    //    {
    //        modeNames.Add(mode.ToString()); // Enum 값을 문자열로 변환하여 추가
    //    }

    //    modeDropdown.AddOptions(modeNames); // 옵션 추가
    //}

    public void ShowCreatRoomUI()
    {
        RoomListArea.SetActive(false); // 룸 리스트는 끄고
        CreatNewRoomArea.SetActive(true); // 룸 생성 UI 켜기

        roomNameInput.text = ""; // 방 이름을 기본적으로 비워둠
        UpdatePlayerCountDisplay(); // 초기 인원 수 표시
        passwordToggle.isOn = false; // 비밀번호 입력 UI를 기본적으로 비활성화
        roomPasswordInput.text = "";
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

    void TogglePasswordInput(bool isOn)
    {
        roomPasswordInput.interactable = isOn;
        SetPasswordArea.gameObject.SetActive(isOn);
    }

    public void CreateRoom()
    {
        // 룸 네임이 입력됐는지 확인 
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            ShowNoticePannel("룸 이름을 를 입력해야 합니다!");
            return; // 방 생성 중단
        }
        
        // 방 옵션 설정
        RoomOptions options = new RoomOptions();
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        options.IsOpen = true;        // 룸의 오픈 여부
        options.IsVisible = true;     // 로비에서 룸 목록 노출 여부

        // 비밀번호가 설정된 경우 체크
        if (passwordToggle.isOn)
        {
            if (string.IsNullOrEmpty(roomPasswordInput.text))
            {
                ShowNoticePannel("비밀번호를 입력해야 합니다!");
                return; // 방 생성 중단
            }
            options.CustomRoomProperties.Add(PhotonRoomProperties.password.ToString(), roomPasswordInput.text);
        }

        // 게임 모드 설정
        //string selectedMode = modeDropdown.options[modeDropdown.value].text;
        //options.CustomRoomProperties.Add(PhotonRoomProperties.mode.ToString(), selectedMode);

        // 최대 인원 수 설정 
        options.MaxPlayers = (byte)currentPlayerCount;

        // 로비에서 표시할 커스텀 속성 설정 -> 로비에서 방 목록을 업데이트할 때 모드와 비밀번호 정보를 포함하게 됨 
        options.CustomRoomPropertiesForLobby = new string[] {
            PhotonRoomProperties.password.ToString()
            //,PhotonRoomProperties.mode.ToString()
        }; 

        // 방 생성 요청
        PhotonManager.Instance.CreateRoom(roomNameInput.text, options);
    }

    public void CreateRoomCancel()
    {
        CreatNewRoomArea.SetActive(false);
        RoomListArea.SetActive(true);
    }
    #endregion

    #region JoinRoom
    private void ShowPasswordPannel(RoomInfo room)
    {
        selectedRoomName = room.Name;
        roomPassword = (string)room.CustomProperties[PhotonRoomProperties.password.ToString()];

        passwordPromptPanel.SetActive(true); // 비밀번호 입력 UI 활성화
        passwordInput.text = "";

        // 비밀번호 확인 버튼 리스너 설정 (중복 방지 위해 기존 리스너 제거 후 추가)
        passwordSubmitButton.onClick.RemoveAllListeners();
        passwordSubmitButton.onClick.AddListener(VerifyPassword);
    }

    private void VerifyPassword()
    {
        if (passwordInput.text == roomPassword)
        {
            Debug.Log("비밀번호 일치! 방 입장 시도!");
            passwordPromptPanel.SetActive(false);
            PhotonManager.Instance.JoinRoomByName(selectedRoomName);
        }
        else
        {
            Debug.LogWarning("비밀번호가 틀렸습니다!");
            passwordInput.text = "";
        }
    }

    private void CancelVerifyPassword()
    {
        passwordPromptPanel.SetActive(false);
        RoomListArea.SetActive(true);
    }
    #endregion


    public void ShowNoticePannel(string message)
    {
        NoticePannel.SetActive(true);
        NoticeText.text = message;
    }
    private void OnDestroy()
    {
        // 이벤트 해제 (메모리 누수 방지)
        PhotonManager.OnLobbyJoined -= ShowRoomListPanel;
        PhotonManager.OnRoomListUpdated -= UpdateRoomList;
        PhotonManager.OnPasswordCheckRequested -= ShowPasswordPannel;
        PhotonManager.OnRoomCreationFailed -= ShowNoticePannel;
    }
}
