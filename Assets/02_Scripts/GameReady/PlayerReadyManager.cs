using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerReadyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool isReady = false; // 내 레디 상태
    
    public static event System.Action OnAllPlayersReady; // 모든 플레이어가 레디 상태가 되었을 때 실행할 이벤트

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

        //Debug.Log($"플레이어 {PhotonNetwork.LocalPlayer.NickName} 레디 상태: {isReady}");

        // 모든 플레이어의 레디 상태 체크 후 게임 시작 가능 여부 확인
        //CheckAllPlayersReady(); // 여기서 실행시키면 SetCustomProperties() 호출 직후 실행되면서, 아직 동기화가 완료되지 않은 상태일 수 있음
    }

    // Photon에서 플레이어 `CustomProperties`가 변경될 때 호출됨
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PhotonPlayerProperties.IsReady.ToString()))
        {
            Debug.Log($"플레이어 {targetPlayer.NickName} 레디 상태 업데이트됨!");
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
            return; // 설정된 최대 인원보다 적으면 게임 시작 불가능!
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey(PhotonPlayerProperties.IsReady.ToString()))
            {
                bool playerReady = (bool)player.CustomProperties[PhotonPlayerProperties.IsReady.ToString()];
                if (!playerReady) return; // 한 명이라도 레디 안 하면 게임 시작 불가능
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName} 레디 정보 없음 ");
                return; // 한 명이라도 레디 정보 없으면 게임 시작 불가능
            }
        }

        Debug.Log("모든 플레이어가 레디 완료 & 최대 인원 충족! 게임 시작 가능!");

        OnAllPlayersReady?.Invoke(); // 이벤트 호출 (모든 플레이어가 레디 상태일 때 실행됨)
    }

}
