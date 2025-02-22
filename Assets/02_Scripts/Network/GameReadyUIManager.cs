using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameReadyUIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameMode gameMode; // TODO : 게임 매니저로 옮기기!! 

    GameReadyNetworkManager gameReadyNetworkManager;

    [Header("RoomSetting")]
    public GameObject RoomSettingArea; // 게임 설정관련 UI 영역 
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

         InitUI();

        // 현재 모드 셋팅 
        if (PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.mode.ToString()] == GameMode.DeathMatch.ToString())
        {
            gameMode = GameMode.DeathMatch;
        }
        else
        {
            gameMode = GameMode.TeamMatch;
        }


        UpdatePlayerListUI();
    }

    void InitUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에게만 룸 셋팅 UI 와 게임 스타트 버튼이 보임 
            RoomSettingArea.SetActive(true);
            GameStartButton.gameObject.SetActive(true);
            GameStartButton.interactable = false;

            // 모드 드롭 다운에 버튼 클릭 리스너 추가 
            modeDropdown.onValueChanged.AddListener(UpdateMaxPlayerSelectUI); // 팀 모드로 선택 할 시 팀 갯수를 선택할 수 있는 드롭 다운 활성화 
            modeDropdown.onValueChanged.AddListener(OnModeDropdownChanged); // 선택된 모드로 포톤 룸 설정 

            MaxPlayerCountDropdown_DeathMatch.onValueChanged.AddListener(OnMaxPlayersChanged);
            MaxPlayerCountDropdown_TeamMatch.onValueChanged.AddListener(OnMaxPlayersChanged);

            GameStartButton.onClick.AddListener(gameReadyNetworkManager.GameStart);

        }
        else {
            // 일반 클라이언트에게는 룸 셋팅 UI 와 게임 스타트 버튼이 보이지 않도록 
            RoomSettingArea.SetActive(false);
            GameStartButton.gameObject.SetActive(false);
        }

    }

    public void UpdatePlayerListUI()
    {
        foreach (Transform child in PlayerInfoListContent)
        {
            Destroy(child.gameObject);
        }

        playerUIObjects.Clear();


        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            GameObject playerItem = Instantiate(PlayerInfoItemPrefab, PlayerInfoListContent);
            playerItem.GetComponentInChildren<TMP_Text>().text = player.Value.NickName;
            playerUIObjects[player.Value.ActorNumber] = playerItem;
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
            TeamCountArea.SetActive(true);
        }
        else
        {
            MaxPlayerCountDropdown_DeathMatch.gameObject.SetActive(true);
            MaxPlayerCountDropdown_TeamMatch.gameObject.SetActive(false);
            TeamCountArea.SetActive(false);
        }
    }

    /// <summary>
    /// 선택된 모드로 포톤 룸 설정 변경 
    /// </summary>
    private void OnModeDropdownChanged(int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string selectedMode = modeDropdown.options[index].text;
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props[RoomProperties.mode.ToString()] = selectedMode;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"게임 모드가 {selectedMode}로 변경됨");
            //Debug.Log($"게임 모드가 {(string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.mode.ToString()]}로 변경됨"); // 이게 반영되는게 시간이 좀 걸리는것 같음

            if (selectedMode == GameMode.DeathMatch.ToString())
            {
                gameMode = GameMode.DeathMatch;
                Debug.Log($"{GameMode.DeathMatch} 으로 변경됨");
            }
            else
            {
                gameMode = GameMode.TeamMatch;
                Debug.Log($"{GameMode.TeamMatch} 으로 변경됨");
            }

            OnMaxPlayersChanged(0); // 최대 플레이어 초기화 
        }
    }

    private void OnMaxPlayersChanged(int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //string selectedMode = (string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.mode.ToString()];
            string selectedMode = gameMode.ToString();
            Debug.Log($"현재 모드 : {selectedMode}");

            // 모드에 따라 최대 인원 수 설정
            int maxPlayers;
            if (selectedMode == GameMode.TeamMatch.ToString())
            {
                maxPlayers = int.Parse(MaxPlayerCountDropdown_TeamMatch.options[index].text);
                UpdateMaxTeamCount(maxPlayers);
            }
            else
            {
                maxPlayers = int.Parse(MaxPlayerCountDropdown_DeathMatch.options[index].text);
            }

            PhotonNetwork.CurrentRoom.MaxPlayers = (byte)maxPlayers;
            Debug.Log($"최대 플레이어 수가 {maxPlayers}로 변경됨");
        }
    }

    /// <summary>
    /// 최대 플레이어 수 변경 시 팀 개수 드롭다운 업데이트
    /// </summary>
    void UpdateMaxTeamCount(int maxPlayer)
    {
        Debug.Log(maxPlayer);

        int maxTeams = maxPlayer / 2; // 최대 팀 개수는 (최대 플레이어 수 / 2)

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
