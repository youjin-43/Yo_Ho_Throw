using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnManager : MonoBehaviourPun, IOnEventCallback
{
    public static PlayerSpawnManager Instance { get; private set; } = null;

    [SerializeField] SpawnPosition[] spawnPositions;

    [SerializeField] GameObject playerObject;

    [SerializeField] Transform mainCamera;
    [SerializeField] CinemachineCamera world_followCam;

    [SerializeField] Transform camaraRoot;


    [SerializeField] float offsetY = 0.5f;

    [SerializeField] float respawnTime = 5f;

    [HideInInspector] public GameObject currPlayer = null;

    [HideInInspector] public PhotonView currPlayerPhotonView = null;

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
            this.spawnPositions[PhotonNetwork.CurrentRoom.PlayerCount - 1].GetRandomTransforms();

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
                    new RaiseEventOptions { TargetActors = new int[] { actorNumber } },
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

        Debug.Log("수정 전 위치 : " + position.ToString());

        position.y = GetHighestCollisionY(position);

        Debug.Log("수정 후 위치 : " + position.ToString());

        // 각자의 클라이언트에서 PhotonNetwork를 통한 Instantiate를 하기 때문에 별도의 RPC는 없어도 된다
        currPlayer = PhotonNetwork.Instantiate(playerObject.name, position, rotation);

        currPlayerPhotonView = currPlayer.GetComponent<PhotonView>();

        currPlayerPhotonView.RPC("GameEndPlayer", RpcTarget.All);

        // 카메라에 루트 셋팅 
        camaraRoot = currPlayer.GetComponent<PlayerController>().CinemachineCameraTarget.transform;
        //currPlayer.GetComponent<PlayerKnifeController>().dirTransform = camaraRoot;
        //currPlayer.GetComponent<PlayerKnifeController>().cameraTransform = world_followCam.transform;

        currPlayer.GetComponent<PhotonView>().RPC("OnOutLobby", RpcTarget.All);

        world_followCam.Target.TrackingTarget = camaraRoot;
        world_followCam.Target.LookAtTarget = camaraRoot;
        world_followCam.Target.CustomLookAtTarget = true;


        InGameUIManager.Instance.Minimap.SetPlayerTransform(currPlayer.transform);

        BattleSystem.SpawnCheck();

        ActivatePlayer();
    }
    [PunRPC]
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerCoroutine());
    }
    IEnumerator RespawnPlayerCoroutine()
    {
        yield return InGameUIManager.Instance.Death(respawnTime);

        BattleUIController.Instance.SetIsAlive(true);

        Transform spawnPosition = GetRandomTransform();

        Vector3 position = spawnPosition.position;
        Quaternion rotation = spawnPosition.rotation;

        position.y = GetHighestCollisionY(position);

        currPlayer.transform.position = spawnPosition.position + Vector3.up * offsetY;
        currPlayer.transform.rotation = spawnPosition.rotation;

        ActivatePlayer();
    }
    [PunRPC]
    public void ActivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        currPlayerPhotonView.RPC("InitPlayer", RpcTarget.All);
    }
    [PunRPC]
    public void DeactivatePlayer()
    {
        // TODO 찬규 : 플레이어 동작 비활성화 (currPlayerPhotonView.RPC를 통해 수행해야할듯)
        currPlayerPhotonView.RPC("GameEndPlayer", RpcTarget.All);
    }
    public void ExecuteRPC(string functionName, int actorNumber)
    {
        photonView.RPC(functionName, PhotonNetwork.CurrentRoom.Players[actorNumber]);
    }
    public void ExecuteRPC(string functionName)
    {
        photonView.RPC(functionName, RpcTarget.All);
    }
    public void ExecutePlayerRPC(string functionName)
    {
        currPlayerPhotonView.RPC(functionName, RpcTarget.All);
    }
    public void EndGame()
    {
        DeactivatePlayer();
    }
    [PunRPC]
    public void ActivateBountyTarget()
    {
        Debug.Log("ActivateBountyTarget 주인 : " + photonView.OwnerActorNr.ToString());

        // TODO 찬규 : 현상금 타겟 지정 이펙트 활성화
        currPlayerPhotonView.RPC("BountyColorSetting", RpcTarget.All);
    }
    [PunRPC]
    public void DeactivateBountyTargetImmediate()
    {
        // TODO 찬규 : 현상금 타겟 지정 이펙트 비활성화
        currPlayerPhotonView.RPC("DefaultColorSetting", RpcTarget.All);
    }
    [PunRPC]
    public void ScheduleBountyTargetDeactivation()
    {
        currPlayerPhotonView.RPC("RespawnColorSetting", RpcTarget.All);
    }
    float GetHighestCollisionY(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 100f, Vector3.down);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 150f);

        Debug.Log("충돌 위치 : " + hit.point.ToString());

        return hit.point.y;
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