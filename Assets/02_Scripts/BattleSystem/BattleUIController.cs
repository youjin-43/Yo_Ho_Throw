using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class BattleUIController : MonoBehaviourPun, IOnEventCallback
{
    public static BattleUIController Instance { get; private set; } = null;

    [Header("플레이어 점수 기록 프리팹")]
    [SerializeField] PlayerScoreEntry playerScoreEntryPrefab;

    [Header("플레이어의 점수 기록 패널 부모 GameObject")]
    [SerializeField] GameObject scoreboardPanel;

    [Header("아이템 선택 대기 화면")]
    [SerializeField] GameObject waitingScreenUI;

    [Header("게임 종료 시 표시할 UI")]
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] Button goToTitleButton;
    [SerializeField] Button goToGameReadyButton;

    [Header("ExitPopup")]
    [SerializeField] GameObject ExitPopup;
    [SerializeField] Button ExitButton;

    [Header("LimitTime")]
    [SerializeField] TMP_Text limitTimeText;

    [Header("순위에 따른 이미지들")]
    [SerializeField] Sprite goldMedalSprite;
    [SerializeField] Sprite silverMedalSprite;
    [SerializeField] Sprite bronzeMedalSprite;

    [Header("전투 시작 텍스트")]
    [SerializeField] GameObject readyTexts;
    [SerializeField] GameObject goTexts;

    [Header("알림 요소")]
    [SerializeField] CanvasGroup notificationCanvasGroup;
    [SerializeField] TMP_Text notificationText;

    Dictionary<int, PlayerScoreEntry> playerScoreEntries = new Dictionary<int, PlayerScoreEntry>();

    List<RealtimePlayerScoreEntry> realtimeScoreEntry = new List<RealtimePlayerScoreEntry>();

    bool isGameRunning = true;
    bool isAlive = true;
    private void Awake()
    {
        Instance = this;

        scoreboardPanel.SetActive(false);

        int i = 0;

        // 모든 플레이어의 ActorNumber를 가져온다
        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            // 탭 누르면 나오는 스코어보드
            {
                playerScoreEntries[actorNumber] =
                    InGameUIManager.Instance.InitScoreboard(actorNumber, PhotonNetwork.CurrentRoom.Players[actorNumber].NickName);
            }


            // 우상단에 상시 표시되는 스코어보드
            {
                // 2. InGameUI 연동
                if (i < 3)
                {
                    realtimeScoreEntry.Add(InGameUIManager.Instance.InitRealtimeScoreboard(
                            i, PhotonNetwork.CurrentRoom.Players[actorNumber].NickName));
                    realtimeScoreEntry[realtimeScoreEntry.Count - 1]._isVisible = true;
                    i++;
                }

            }
        }

        isGameRunning = true;

        // GameOverPopup
        gameoverPanel.SetActive(false);
        goToTitleButton.onClick.AddListener(()=>PhotonManager.Instance.LeaveRoomAndGoToTitle());
        goToGameReadyButton.onClick.AddListener(() => PhotonManager.Instance.GoToReadyScene()); // TODO : 포톤 네트워크 잘 연결 되는지 확인 필요

        // ExitPopup
        ExitPopup.SetActive(false);
        ExitButton.onClick.AddListener(() => PhotonManager.Instance.LeaveRoomAndGoToTitle());
    }

    private void Update()
    {
        if (!isGameRunning) return;

        if (Input.GetKeyDown(KeyCode.Tab) && isAlive)
        {
            ShowScoreboard();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }

        // ESC키로 ExitPopup 토글

        // ㅇㅎㅈ 추가 : 설정창이 떠있는 상태에서는 메뉴창(ExitPopup) 토글이 막혀야 할듯

        if (Input.GetKeyDown(KeyCode.Escape) && InGameUIManager.Instance.IsSettingUIOpen() == false)
        {
            InGameUIManager.Instance.ToggleMenuUI();
        }
        
        if(Input.GetKeyDown(KeyCode.Escape) && InGameUIManager.Instance.IsSettingUIOpen() == true)
        {
            InGameUIManager.Instance.ToggleSettingUI();
        }
    }

    /// <summary>
    /// ExitPopup 토글 및 마우스 상태 변경
    /// </summary>
    private void ToggleExitPopup()
    {
        bool isActive = !ExitPopup.activeSelf;
        ExitPopup.SetActive(isActive);

        if (isActive)
        {
            CursorController.Instance.CursorEnable(); // 마우스 활성화
            GameManager.Instance.PlayerStop(true);
        }
        else
        {
            CursorController.Instance.CursorDisable(); // 마우스 비활성화
            GameManager.Instance.PlayerStop(false);
        }
    }

    public void SetIsAlive(bool isAlive)
    {
        this.isAlive = isAlive;

        if (isAlive == false)
        {
            HideScoreboard();
        }
    }
    void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);
    }
    void HideScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }
    public void OnEvent(EventData photonEvent)
    {
        RaiseEventCode raiseEventCode = (RaiseEventCode)photonEvent.Code;

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateKillCount:
            case RaiseEventCode.UpdateDeathCount:
            case RaiseEventCode.UpdateScore:
                UpdatePlayerScoreEntry(photonEvent, raiseEventCode); break;

            case RaiseEventCode.ResetScoreboard:
                ResetScoreboard(); break;

            case RaiseEventCode.UpdateRank:
                UpdateRank(photonEvent); break;

            case RaiseEventCode.UpdateRealtime:
                UpdateRealtime(photonEvent); break;
        }
    }
    void UpdateRealtime(EventData photonEvent)
    {
        int[][] scoreList = (int[][])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            // 1. 기존 로직
            {
                realtimeScoreEntry[i].gameObject.SetActive(true);

                realtimeScoreEntry[i].SetNickName(PhotonNetwork.CurrentRoom.Players[scoreList[i][0]].NickName);

                realtimeScoreEntry[i].SetRank(scoreList[i][1]);

                realtimeScoreEntry[i].SetScore(scoreList[i][2]);
            }

            // 2. InGameUI 연동
            // InGameUIManager.Instance.UpdateScoreHUDData(i, PhotonNetwork.CurrentRoom.Players[scoreList[i][0]].NickName, scoreList[i][1], scoreList[i][2]);
        }
    }
    void UpdateRank(EventData photonEvent)
    {
        int[][] scoreList = (int[][])photonEvent.CustomData;

        for (int i = 0; i < scoreList.Length; i++)
        {
            playerScoreEntries[scoreList[i][0]].transform.SetAsLastSibling();

            switch (scoreList[i][1])
            {
                case 1:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(goldMedalSprite); break;
                case 2:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(silverMedalSprite); break;
                case 3:
                    playerScoreEntries[scoreList[i][0]].SetIconImage(bronzeMedalSprite); break;
            }
        }

        // 게임 종료 시의 UI 표시
        EndGame();
    }
    void UpdatePlayerScoreEntry(EventData photonEvent, RaiseEventCode raiseEventCode)
    {
        int[] data = (int[])photonEvent.CustomData;

        // 수정할 대상의 ActorNumber와 변경 값 가져오기
        int actorNumber = data[0];
        int value = data[1];

        switch (raiseEventCode)
        {
            case RaiseEventCode.UpdateDeathCount:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetDeathCount(value); break;

                // 2. InGameUI 연동
                // InGameUIManager.Instance.UpdateScoreboardData_DeathCount(actorNumber, value);

            case RaiseEventCode.UpdateScore:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetScore(value); 

                SortPlayerScoreEntry(); break;

            // 2. InGameUI 연동
            // InGameUIManager.Instance.UpdateScoreboardData_Score(actorNumber, value);

            case RaiseEventCode.UpdateKillCount:
                // 1. 기존 로직
                playerScoreEntries[actorNumber].SetKillCount(value); break;

                // 2. InGameUI 연동
                // InGameUIManager.Instance.UpdateScoreboardData_KillCount(actorNumber, value);
        }
    }
    public void EndGame()
    {
        isGameRunning = false;

        //ShowScoreboard();

        //CursorController.Instance.CursorEnable();

        //gameoverPanel.SetActive(true);
    }
    void ResetScoreboard()
    {
        foreach (PlayerScoreEntry playerScoreEntry in playerScoreEntries.Values)
        {
            playerScoreEntry.ResetPlayerScoreEntry();
        }
    }
    [PunRPC]
    public void SetLimitTimeText(int seconds)
    {
        limitTimeText.text = $"{(seconds / 60).ToString()} : {(seconds % 60).ToString("00")}";
    }
    public void SetComboKill(int combo)
    {
        InGameUIManager.Instance.SetCombokill(combo);
    }
    public void SetBattleStartText(int count)
    {
        switch (count)
        {
            case 2: readyTexts.SetActive(true);
                goTexts.SetActive(false); break;

            case 1:
                readyTexts.SetActive(false);
                goTexts.SetActive(true); break;

            case 0:
                readyTexts.SetActive(false);
                goTexts.SetActive(false); break;

        }
    }
    IEnumerator HideTextAfterTimeCoroutine(TMP_Text targetTextUI, float delay)
    {
        yield return new WaitForSeconds(delay);

        targetTextUI.text = string.Empty;

        // 카운트 끝나고 조준점 표시
        InGameUIManager.Instance.ToggleCrosshair(true);
    }
    void SortPlayerScoreEntry()
    {
        var sortedList = playerScoreEntries.Values
                                       .OrderByDescending(entry => entry.score)
                                       .ToList();

        foreach (PlayerScoreEntry entry in sortedList)
        {
            entry.transform.SetAsLastSibling();
        }
    }
    public void ShowWaitingScreen() => waitingScreenUI.SetActive(true);
    public void HideWaitingScreen()
    {
        photonView.RPC("HideWaitingScreenRPC", RpcTarget.All);
    }
    [PunRPC]
    public void HideWaitingScreenRPC()
    {
        waitingScreenUI.SetActive(false);
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    Coroutine notificationCoroutine = null;
    [PunRPC]
    public void Notification(string str)
    {
        notificationText.text = str;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(NotificationCoroutine());
    }
    IEnumerator NotificationCoroutine()
    {
        float fadeSpeed = 5f;
        float duration = 2f;

        float t = 0;

        while(t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;

            notificationCanvasGroup.alpha = t;

            yield return null;
        }

        yield return new WaitForSeconds(duration);

        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;

            notificationCanvasGroup.alpha = t;

            yield return null;
        }
    }
    
    [SerializeField] Slider potionSelectRemainingTime;
    Coroutine selectRandomPotionAfterDelayCoroutine = null;
    public void SelectRandomPotionAfterDelay()
    {
        if (selectRandomPotionAfterDelayCoroutine != null)
            StopCoroutine(selectRandomPotionAfterDelayCoroutine);

        selectRandomPotionAfterDelayCoroutine = StartCoroutine(SelectRandomPotionAfterDelayCoroutine());
    }
    public void StopRandomPotionSelect()
    {
        if (selectRandomPotionAfterDelayCoroutine != null)
        {
            StopCoroutine(selectRandomPotionAfterDelayCoroutine);
        }
    }
    public IEnumerator SelectRandomPotionAfterDelayCoroutine()
    {
        float t = 5f;
        
        while (t > 0f)
        {
            t -= Time.deltaTime;

            potionSelectRemainingTime.value = t;

            yield return null;
        }

        potionSelectRemainingTime.value = 0;

        selectRandomPotionAfterDelayCoroutine = null;

        InGameUIManager.Instance.ItemSelect.ItemSelected(Random.Range(0, 2));
    }
}