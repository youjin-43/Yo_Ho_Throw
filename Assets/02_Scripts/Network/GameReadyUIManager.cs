using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameReadyUIManager : MonoBehaviour
{
    GameReadyNetworkManager gameReadyNetworkManager;

    [Header("RoomSetting")]
    public TMP_Dropdown modeDropdown; // 게임 모드 선택 드롭다운 (개인전/팀전)
    public TMP_Dropdown MaxPlayerCountDropdown_DeathMatch;
    public TMP_Dropdown MaxPlayerCountDropdown_TeamMatch;
    public TMP_Dropdown teamCountDropdown; // 팀 개수 선택 드롭다운
    public GameObject TeamCountArea; // 팀 개수 설정 UI 영역

    [Header("DeathMatch PlayerList")]
    [SerializeField] GameObject PlayerInfoItemPrefab; // 플레이어 인포 프리팹
    public Transform PlayerInfoListContent; // 플레이어 목록이 추가될 부모 오브젝트

    [Header("Team Player List")]
    public Transform TeamContainer;
    public GameObject TeamPanelPrefab; // 팀별 패널 프리팹
    private Dictionary<int, GameObject> playerUIObjects = new Dictionary<int, GameObject>();

    [SerializeField] public Button GameStartButton; // 게임 스타트 버튼 

    private void Start()
    {
        gameReadyNetworkManager = GetComponent<GameReadyNetworkManager>();

        modeDropdown.onValueChanged.AddListener(UpdateMaxPlayerSelectUI);
        modeDropdown.onValueChanged.AddListener((int modeIndex) =>
        {
            string selectedMode = modeDropdown.options[modeIndex].text;
            gameReadyNetworkManager.SetGameMode(selectedMode);
        });

        MaxPlayerCountDropdown_TeamMatch.onValueChanged.AddListener(UpdateMaxTeamCount);
        MaxPlayerCountDropdown_TeamMatch.onValueChanged.AddListener((int maxPlayersIndex) =>
        {
            int maxPlayers = 4 + (int)Math.Pow(2, maxPlayersIndex); // 최소 4부터 시작
            gameReadyNetworkManager.SetMaxPlayers(maxPlayers);
        });

        GameStartButton.onClick.AddListener(gameReadyNetworkManager.GameStart);

        UpdatePlayerListUI();
        GameStartButton.interactable = false;
    }

    public void UpdatePlayerListUI()
    {
        foreach (Transform child in PlayerInfoListContent)
        {
            Destroy(child.gameObject);
        }

        playerUIObjects.Clear();

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            GameObject playerItem = Instantiate(PlayerInfoItemPrefab, PlayerInfoListContent);
            playerItem.GetComponentInChildren<TMP_Text>().text = player.NickName;
            playerUIObjects[player.ActorNumber] = playerItem;
        }

        GameStartButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers; //현재 인원이 MaxPlayers에 도달해야 GameStartButton 활성화
    }

    /// <summary>
    /// 팀 모드 선택 시 팀 개수 UI 활성화
    /// </summary>
    void UpdateMaxPlayerSelectUI(int modeIndex)
    {
        string selectedMode = modeDropdown.options[modeIndex].text;

        if (selectedMode == GameMode.TeamMatch.ToString())
        {
            MaxPlayerCountDropdown_DeathMatch.gameObject.SetActive(false);
            MaxPlayerCountDropdown_TeamMatch.gameObject.SetActive(true);
            UpdateMaxTeamCount(MaxPlayerCountDropdown_TeamMatch.value); // 팀 개수 업데이트
            TeamCountArea.SetActive(true);
        }
        else
        {
            MaxPlayerCountDropdown_DeathMatch.gameObject.SetActive(true);
            MaxPlayerCountDropdown_TeamMatch.gameObject.SetActive(false);
            TeamCountArea.SetActive(false);
        }
    }

    // 최대 플레이어 수 변경 시 팀 개수 드롭다운 업데이트
    void UpdateMaxTeamCount(int maxPlayersIndex)
    {
        int maxPlayers = 4 + (int)Math.Pow(2, maxPlayersIndex); // 최소 4부터 시작
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
}
