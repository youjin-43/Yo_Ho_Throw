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


            InGameUIManager.Instance.OnHealthChangedDebug(hp);
        }
    }

    bool isInLobby = false;

    [PunRPC]
    public void ReceiveDamage(int attackerActorNr, int damage)
    {
        if (isInLobby) return;

        Debug.Log("맞은 플레이어 : " + photonView.OwnerActorNr.ToString());

        Debug.Log("맞기 전 HP : " + Hp.ToString());

        Hp -= damage;

        Debug.Log("맞은 후 HP : " + Hp.ToString());

        if (Hp <= 0)
        {
            photonView.RPC("HandleDeath", RpcTarget.All, attackerActorNr);

            HandleDeath(attackerActorNr);
        }
    }
    [PunRPC]
    void HandleDeath(int killerActorNr)
    {
        // 이동 비활성화
        EditPlayerController.Instance.DisableMovement();

        BattleSystem.Instance.RegisterKill(killerActorNr, photonView.OwnerActorNr);
    }

    public void OnInLobby() => isInLobby = true;
    public void OnOutLobby() => isInLobby = false;
}
