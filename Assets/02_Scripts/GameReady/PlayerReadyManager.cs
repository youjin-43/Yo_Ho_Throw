using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerReadyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool isReady = false; // 내 레디 상태

    public static event System.Action<int,bool> OnPlayerReadyChanged; // 특정 플레이어의 레디 상태가 변경될 때 실행될 이벤트 (ActorNumber 전달)
    public static event System.Action<bool> OnAllPlayersReadyChanged; // 모든 플레이어의 레디 상태 변경 이벤트 (버튼 활성화/비활성화)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleReadyState();
        }
    }

    void ToggleReadyState()
    {
        isReady = !isReady; // 레디 상태 토글

        // Photon CustomProperties에 저장 (모든 플레이어에게 동기화됨)
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties[PhotonPlayerProperties.IsReady.ToString()] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }

    // Photon에서 플레이어 `CustomProperties`가 변경될 때 호출됨
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        string isReadyKey = PhotonPlayerProperties.IsReady.ToString();

        if (changedProps.ContainsKey(isReadyKey))
        {
            bool isReady = (bool)changedProps[isReadyKey]; 
            Debug.Log($"플레이어 {targetPlayer.NickName} 레디 상태 업데이트됨! → {isReady}");

            // 이벤트 발생 → GameReadyUIManager에서 이걸 듣고 UI 업데이트!
            OnPlayerReadyChanged?.Invoke(targetPlayer.ActorNumber, isReady);

            CheckAllPlayersReady();
        }
    }

    /// <summary>
    /// 모든 플레이어의 레디 상태 체크 후 게임 시작 가능 여부 확인
    /// </summary>
    void CheckAllPlayersReady()
    {
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers; // 설정된 최대 플레이어 수
        int currentPlayers = PhotonNetwork.PlayerList.Length; // 현재 방에 있는 플레이어 수

        if (currentPlayers < maxPlayers)
        {
            Debug.Log($"현재 인원({currentPlayers}/{maxPlayers})이 설정된 최대 인원보다 적음");
            OnAllPlayersReadyChanged?.Invoke(false); // 버튼 비활성화 이벤트 호출
            return; // 설정된 최대 인원보다 적으면 게임 시작 불가능!
        }

        bool allReady = true; // 모든 플레이어가 레디 상태인지 체크하는 플래그
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey(PhotonPlayerProperties.IsReady.ToString()))
            {
                bool playerReady = (bool)player.CustomProperties[PhotonPlayerProperties.IsReady.ToString()];
                if (!playerReady)
                {
                    Debug.Log($"{player.NickName}이(가) 레디를 해제함 → 게임 시작 불가!");
                    allReady = false;
                    break;
                }
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName} 레디 정보 없음 → 게임 시작 불가! ");
                allReady = false;
                return; // 한 명이라도 레디 정보 없으면 게임 시작 불가능
            }
        }

        Debug.Log($"모든 플레이어 레디 상태: {allReady}");
        OnAllPlayersReadyChanged?.Invoke(allReady); // UI 매니저에 이벤트 전달
    }
}
