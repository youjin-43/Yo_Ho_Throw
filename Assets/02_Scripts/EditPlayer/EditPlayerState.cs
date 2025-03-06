using Photon.Pun;
using UnityEngine;

public class EditPlayerState : MonoBehaviourPun, IDamagable
{
    const int MAX_HP = 3;

    int hp = MAX_HP;

    int Hp
    {
        get => hp;

        set
        {
            hp = value;

            InGameUIManager.Instance.HealthIndicator.SetHealth(hp);
        }
    }

    bool isInLobby = false;

    [PunRPC]
    public void ReceiveDamage(int attackerActorNr, int damage)
    {
        if (isInLobby || !photonView.IsMine) return;

        if (PhotonNetwork.LocalPlayer.ActorNumber != photonView.OwnerActorNr) return;

        Hp -= damage;

        if (Hp <= 0)
        {
            photonView.RPC("HandleDeath", RpcTarget.All, attackerActorNr);
        }
    }
    [PunRPC]
    void HandleDeath(int killerActorNr)
    {
        if (!photonView.IsMine) return;

        gameObject.name += Random.value.ToString();

        // 이동 비활성화
        EditPlayerController.Instance.DisableMovement();

        BattleSystem.Instance.photonView.RPC("RegisterKillRPC", RpcTarget.All, killerActorNr, photonView.OwnerActorNr);
    }
    //[PunRPC]
    //public void OnInLobby()
    //{
    //    GetComponent<PlayerKnifeController>().OnInLobby();
    //    GetComponent<EditPlayerState>().OnInLobby();
    //}
    //[PunRPC]
    //public void OnOutLobby()
    //{
    //    GetComponent<PlayerKnifeController>().OnOutLobby();
    //    GetComponent<EditPlayerState>().OnOutLobby();
    //}
    
}
