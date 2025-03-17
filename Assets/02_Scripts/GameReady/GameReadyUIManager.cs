using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameReadyUIManager : MonoBehaviourPunCallbacks
{
    GameReadyNetworkManager gameReadyNetworkManager;

    [SerializeField] GameObject PlayerInfoItemPrefab; // 플레이어 인포 프리팹
    [SerializeField] Transform PlayerInfoListContent; // 플레이어 목록이 추가될 부모 오브젝트
    [SerializeField] Button GameStartButton; // 게임 스타트 버튼
    [SerializeField] Button GameReadyButton; // 게임 준비 버튼
    [SerializeField] GameObject ExitPopup; 
    [SerializeField] Button goToTitleButton;
    [SerializeField] Button stayButton;

    [SerializeField] private TextMeshProUGUI plyaerJoinLeaveText; // 누가 들어오고 나갔는지 


    // Player Info
    string currentScene;
    Dictionary<int, GameObject> playerUIObjects = new Dictionary<int, GameObject>(); // 현재 입장한 플레이어들

    enum PIChild
    {
        ProfileArea,
        Name,
        IsReady,
        IsMaster
    }

    private void Start()
    {
        gameReadyNetworkManager = GetComponent<GameReadyNetworkManager>();
        currentScene = SceneManager.GetActiveScene().name;

        // 이벤트 구독 
        PlayerReadyManager.OnPlayerReadyChanged += SetReady; //플레이어가 레디 상태가 변경될 때 UI 업데이트
        PlayerReadyManager.OnAllPlayersReadyChanged += SetGameStartButtonInteractable; // 모든 플레이어가 레디 상태가 되면 게임 스타트 버튼 활성화 

        GameReadyNetworkManager.OnPlayerEnter += HandlePlayerEnter; // 플레이어가 들어왔을 떄 이벤트 
        GameReadyNetworkManager.OnPlayerLeft += HandlePlayerLeft; // 플레이어가 나갔을때 이벤트

        InitUI();
        UpdatePlayerListUI();

        ScreenTransition.Instance.FadeInRPC();
    }

    void Update()
    {
        // Tab 키로 ExitPopup 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPopup();
        }

        // F5 키 입력 시 게임 시작 (마스터 클라이언트만 가능)
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.F5))
        {
            if (GameStartButton.interactable)
            {
                ScreenTransition.Instance.FadeActionSetting(() => gameReadyNetworkManager.GameStart());

                ScreenTransition.FadeOut();

                GameStartButton.interactable = false;
            }
        }
    }

    void InitUI()
    {
        SetReadyAndStartUI();

        // Exit 팝업 기본 비활성화
        ExitPopup.SetActive(false);

        // 버튼 클릭 리스너 설정
        goToTitleButton.onClick.AddListener(() => PhotonManager.Instance.LeaveRoomAndGoToTitle());
        stayButton.onClick.AddListener(ToggleExitPopup);
    }

    /// <summary>
    /// ExitPopup 토글 및 마우스 상태 변경
    /// </summary>
    private void ToggleExitPopup()
    {
        bool isActive = !ExitPopup.activeSelf;
        ExitPopup.SetActive(isActive);

        if (isActive) CursorController.Instance.CursorEnable(); // 마우스 활성화
        else CursorController.Instance.CursorDisable(); // 마우스 비활성화
    }

    public void SetReadyAndStartUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에게만 게임 스타트 버튼이 보임
            GameReadyButton.gameObject.SetActive(false);
            GameStartButton.gameObject.SetActive(true);
            GameStartButton.interactable = false;
            //GameStartButton.onClick.AddListener(gameReadyNetworkManager.GameStart); // 게임 스타트 리스너 추가 
        }
        else
        {
            // 일반 클라이언트에게는 게임 스타트 버튼이 보이지 않도록 
            GameStartButton.gameObject.SetActive(false);
            GameReadyButton.gameObject.SetActive(true);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetReadyAndStartUI();
    }

    // GameReadyNetworkManager의 OnPlayerEnteredRoom에서 호출됨 
    public void UpdatePlayerListUI()
    {
        // 기존 데이터 삭제 
        foreach (Transform child in PlayerInfoListContent) Destroy(child.gameObject);
        playerUIObjects.Clear();

        // 플레이어마다 새롭게 데이터 추가 
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");

            GameObject playerItem = Instantiate(PlayerInfoItemPrefab, PlayerInfoListContent);

            // 닉네임 설정 
            playerItem.transform.GetChild((int)PIChild.Name).GetComponentInChildren<TMP_Text>().text = player.Value.NickName;

            // 마스터인지 확인 -> 왕관으로 표시 
            if (player.Value.IsMasterClient)
            {
                playerItem.transform.GetChild((int)PIChild.IsMaster).gameObject.SetActive(true);
                playerItem.transform.GetChild((int)PIChild.IsReady).gameObject.SetActive(false);
            }
            else
            {
                // 일반 클라이언트라면 포톤 커스텀 프로퍼티에서 `IsReady` 값 확인하여 값에 따라 Ready,Crown UI 활성화/비활성화
                bool isReady = false;
                if (player.Value.CustomProperties.ContainsKey(PhotonPlayerProperties.IsReady.ToString()))
                {
                    // 왕관 비활성화 
                    playerItem.transform.GetChild((int)PIChild.IsMaster).gameObject.SetActive(false);

                    // 레디 확인 
                    isReady = (bool)player.Value.CustomProperties[PhotonPlayerProperties.IsReady.ToString()];
                    playerItem.transform.GetChild((int)PIChild.IsReady).gameObject.SetActive(isReady);
                }
            }

            // 플레이어가 어디씬에 있는지 확인 
            //string playerScene = player.Value.CustomProperties.ContainsKey("CurrentScene")
            //  ? (string)player.Value.CustomProperties["CurrentScene"]
            //  : "Unknown";

            //Image panelBackground = playerItem.GetComponent<Image>();
            //if (playerScene == currentScene) panelBackground.color = new Color(1f, 1f, 1f, 1f); // 현재 씬 플레이어 → 불투명
            //else panelBackground.color = new Color(1f, 1f, 1f, 0.5f); // 다른 씬 플레이어 → 투명 처리

            // 딕셔너리에 등록 
            playerUIObjects[player.Value.ActorNumber] = playerItem;
        }
    }

    // PlayerReadyManager의 OnPlayerPropertiesUpdate에서 실행 
    public void SetReady(int playerActorNumber, bool isrReady)
    {
        playerUIObjects[playerActorNumber].transform.GetChild((int)PIChild.IsReady).gameObject.SetActive(isrReady);
    }

    /// <summary>
    /// 모든 플레이어가 레디 상태가 되었을 때 버튼 활성화/비활성화
    /// </summary>
    private void SetGameStartButtonInteractable(bool isInteractable)
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 버튼 조작 가능
        {
            GameStartButton.interactable = isInteractable;
        }
    }



    private void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        PlayerReadyManager.OnPlayerReadyChanged -= SetReady;
        PlayerReadyManager.OnAllPlayersReadyChanged -= SetGameStartButtonInteractable;
        GameReadyNetworkManager.OnPlayerEnter -= HandlePlayerEnter; 
        GameReadyNetworkManager.OnPlayerLeft -= HandlePlayerLeft;
    }


    #region plyaer Join Leave
    public void ShowPlayerLeftMessage(string playerName)
    {
        if (plyaerJoinLeaveText == null) return; // 메시지 텍스트가 없으면 무시

        plyaerJoinLeaveText.text = $"{playerName}님이 나갔습니다!";
        plyaerJoinLeaveText.GetComponentInParent<Animator>().SetTrigger("Show");

        HideMessage();
    }

    public void ShowPlayerJoinMessage(string playerName)
    {
        if (plyaerJoinLeaveText == null) return; // 메시지 텍스트가 없으면 무시

        plyaerJoinLeaveText.text = $"{playerName}님이 입장했습니다!";
        plyaerJoinLeaveText.GetComponentInParent<Animator>().SetTrigger("Show");

        HideMessage();
    }

    void HideMessage()
    {
        // 2초 후 메시지 자동 사라짐
        Invoke(nameof(ShowHideAnim), 2f);
    }

    void ShowHideAnim()
    {
        plyaerJoinLeaveText.GetComponentInParent<Animator>().SetTrigger("Hide");
    }

    private void HandlePlayerEnter(string playerName)
    {
        UpdatePlayerListUI();
        ShowPlayerJoinMessage(playerName);
    }

    private void HandlePlayerLeft(string playerName)
    {
        UpdatePlayerListUI();
        ShowPlayerLeftMessage(playerName);
    }

    #endregion


}
