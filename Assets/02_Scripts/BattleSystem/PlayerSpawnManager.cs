using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour, IOnEventCallback
{
    public static PlayerSpawnManager Instance { get; private set; } = null;

    [SerializeField] List<Transform> spawnPositions;

    [SerializeField] GameObject playerObject;

    GameObject currPlayer = null;
    PhotonView currPlayerPhotonView = null;

    private void Awake()
    {
        Instance = this;
    }
    public IEnumerator SpawnCoroutine()
    {
        // 호스트가 아니라면 코루틴을 빠져나온다
        if (!PhotonNetwork.IsMasterClient) yield break;

        // TODO 찬규 : 각 플레이어의 연결 상태를 파악할 필요가 있을 지 고민 더 해야함
        /* while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("호스트가 연결되어 있지 않음");
            yield return null;
        }*/

        List<Transform> spawnPositions = GetRandomTransforms();

        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            int index = Random.Range(0, spawnPositions.Count);

            Transform spawnPosition = spawnPositions[index];

            spawnPositions.RemoveAt(index);

            // 타 플레이어의 플레이어 오브젝트 생성 시
            if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
            {
                PhotonNetwork.RaiseEvent(
                    (byte)RaiseEventCode.SpawnPlayer,
                    (Transform)spawnPosition,
                    new RaiseEventOptions { TargetActors = new int[]{ actorNumber } },
                    SendOptions.SendReliable);
            }

            // 호스트의 플레이어 오브젝트 생성 시
            else
            {
                SpawnPlayer(spawnPosition);
            }
        }

    }
    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.SpawnPlayer:
                SpawnPlayer(photonEvent); break;

            case RaiseEventCode.RespawnPlayer:
                RespawnPlayer(); break;

            case RaiseEventCode.ActivatePlayer:
                ActivatePlayer(); break;

            case RaiseEventCode.DeactivatePlayer:
                DeactivatePlayer(); break;
        }
    }
    void SpawnPlayer(EventData photonEvent)
    {
        Transform spawnPosition = (Transform)photonEvent.CustomData;

        SpawnPlayer(spawnPosition);
    }
    void SpawnPlayer(Transform spawnPosition)
    {
        // 각자의 클라이언트에서 PhotonNetwork를 통한 Instantiate를 하기 때문에 별도의 RPC는 없어도 된다
        currPlayer = PhotonNetwork.Instantiate(playerObject.name, spawnPosition.position, spawnPosition.rotation);

        currPlayerPhotonView = currPlayer.GetComponent<PhotonView>();

        ActivatePlayer();
    }
    void RespawnPlayer()
    {
        Transform spawnPosition = GetRandomTransform();

        currPlayer.transform.position = spawnPosition.position;
        currPlayer.transform.rotation = spawnPosition.rotation;

        ActivatePlayer();
    }
    void ActivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        //currPlayerPhotonView.RPC("")
    }
    void DeactivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 비활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        
    }
    public void EndGame()
    {
        DeactivatePlayer();
    }
    List<Transform> GetRandomTransforms()
    {
        if (this.spawnPositions.Count < PhotonNetwork.CurrentRoom.Players.Count)
        {
            Debug.LogWarning($"스폰 가능한 위치보다 플레이어 수가 더 많음 ! ({this.spawnPositions.Count} < {PhotonNetwork.CurrentRoom.Players.Count})");

            return null;
        }

        List<Transform> spawnPositions = this.spawnPositions;

        while (spawnPositions.Count > PhotonNetwork.CurrentRoom.Players.Count)
        {
            spawnPositions.RemoveAt(Random.Range(0, spawnPositions.Count));
        }

        return spawnPositions;
    }
    Transform GetRandomTransform() => spawnPositions[Random.Range(0, spawnPositions.Count)];
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}
