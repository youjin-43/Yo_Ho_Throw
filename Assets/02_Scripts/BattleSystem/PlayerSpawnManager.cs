using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnManager : MonoBehaviourPun, IOnEventCallback
{
    public static PlayerSpawnManager Instance { get; private set; } = null;

    [SerializeField] SpawnPosition[] spawnPositions;

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

        List<Transform> spawnPositions =
            this.spawnPositions[PhotonNetwork.CurrentRoom.PlayerCount-1].GetRandomTransforms();

        foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            Transform spawnPosition = spawnPositions[0];

            spawnPositions.RemoveAt(0);

            // 타 플레이어의 플레이어 오브젝트 생성 시
            if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
            {
                PhotonNetwork.RaiseEvent(
                    (byte)RaiseEventCode.SpawnPlayer,
                    new object[] { spawnPosition.position, spawnPosition.rotation },
                    new RaiseEventOptions { TargetActors = new int[]{ actorNumber } },
                    SendOptions.SendReliable);
            }

            // 호스트의 플레이어 오브젝트 생성 시
            else
            {
                SpawnPlayer(new object[] { spawnPosition.position, spawnPosition.rotation });
            }
        }
    }
    public void OnEvent(EventData photonEvent)
    {
        switch ((RaiseEventCode)photonEvent.Code)
        {
            case RaiseEventCode.SpawnPlayer:
                SpawnPlayer(photonEvent); break;
        }
    }
    void SpawnPlayer(EventData photonEvent)
    {
        SpawnPlayer((object[])photonEvent.CustomData);
    }
    void SpawnPlayer(object[] spawnInfo)
    {
        Vector3 position = (Vector3)spawnInfo[0];
        Quaternion rotation = (Quaternion)spawnInfo[1];

        // TODO 찬규 : 확실한 테스트를 위한 Pivot 조정 (제거 or 수정 바람)
        position += Vector3.up;

        // 각자의 클라이언트에서 PhotonNetwork를 통한 Instantiate를 하기 때문에 별도의 RPC는 없어도 된다
        currPlayer = PhotonNetwork.Instantiate(playerObject.name, position, rotation);

        currPlayerPhotonView = currPlayer.GetComponent<PhotonView>();

        ActivatePlayer();
    }
    [PunRPC]
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerCoroutine());
    }
    IEnumerator RespawnPlayerCoroutine()
    {
        // TODO 찬규 : 리스폰 수정

        int remainSpawnTimer = 3;

        while (remainSpawnTimer-- > 0)
        {
            yield return new WaitForSeconds(1f);

            BattleUIController.Instance.SetRespawnTimer(remainSpawnTimer);
        }

        BattleUIController.Instance.SetRespawnTimer(remainSpawnTimer);

        Transform spawnPosition = GetRandomTransform();

        currPlayer.transform.position = spawnPosition.position;
        currPlayer.transform.rotation = spawnPosition.rotation;

        ActivatePlayer();
    }
    [PunRPC]
    public void ActivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        //currPlayerPhotonView.RPC("")
    }
    [PunRPC]
    public void DeactivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 비활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        
    }
    public void ExecuteRPC(string functionName, int actorNumber)
    {
        photonView.RPC(functionName, PhotonNetwork.CurrentRoom.Players[actorNumber]);
    }
    public void EndGame()
    {
        DeactivatePlayer();
    }
    [PunRPC]
    public void ActivateBountyTarget()
    {
        // TODO 찬규 : 현상금 타겟 지정 이펙트 활성화

    }
    [PunRPC]
    public void DeactivateBountyTarget()
    {
        // TODO 찬규 : 현상금 타겟 지정 이펙트 비활성화

    }
    Transform GetRandomTransform() => spawnPositions[Random.Range(0, spawnPositions.Length)].GetRandomTransform();
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.AddCallbackTarget(this);
}

[Serializable]
public struct SpawnPosition
{
    public Transform[] positions;
    public List<Transform> GetRandomTransforms()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount > this.positions.Length)
        {
            Debug.LogWarning("PlayerSpawnManager에 등록되어 있는 포지션의 수보다 현재 플레이어 수가 더 많습니다");

            return null;
        }

        List<Transform> positions = this.positions.ToList();

        while (positions.Count > playerCount)
        {
            positions.RemoveAt(Random.Range(0, positions.Count));
        }

        for (int i = 0; i < positions.Count; i++)
        {
            int randomIndex = Random.Range(0, positions.Count - 1);

            (positions[i], positions[randomIndex]) = (positions[randomIndex], positions[i]);
        }

        return positions;
    }
    public Transform GetRandomTransform()
    {
        return positions[Random.Range(0, positions.Length)];
    }
    //
}