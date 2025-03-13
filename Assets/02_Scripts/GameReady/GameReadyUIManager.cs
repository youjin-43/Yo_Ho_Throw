using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    private Dictionary<int, GameObject> playerUIObjects = new Dictionary<int, GameObject>(); // 현재 입장한 플레이어들

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

        // 이벤트 구독 
        PlayerReadyManager.OnPlayerReadyChanged += SetReady; //플레이어가 레디 상태가 변경될 때 UI 업데이트
        PlayerReadyManager.OnAllPlayersReadyChanged += SetGameStartButtonInteractable; // 모든 플레이어가 레디 상태가 되면 게임 스타트 버튼 활성화 

        InitUI();
        UpdatePlayerListUI();
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
                Debug.Log("F5 키 입력 감지 → 게임 시작!");
                gameReadyNetworkManager.GameStart();
            }
        }
    }

    void InitUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에게만 게임 스타트 버튼이 보임
            GameReadyButton.gameObject.SetActive(false);
            GameStartButton.gameObject.SetActive(true);
            GameStartButton.interactable = false;
            GameStartButton.onClick.AddListener(gameReadyNetworkManager.GameStart); // 게임 스타트 리스너 추가 
        }
        else {
            // 일반 클라이언트에게는 게임 스타트 버튼이 보이지 않도록 
            GameStartButton.gameObject.SetActive(false);
            GameReadyButton.gameObject.SetActive(true);
        }

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
    }
}
