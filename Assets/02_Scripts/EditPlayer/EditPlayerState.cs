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
    public void ReceiveDamage(int attackerActorNumber, int damage)
    {
        if (isInLobby) return;

        Hp -= damage;

        if (Hp <= 0)
        {
            HandleDeath(attackerActorNumber);
        }
    }
    void HandleDeath(int killerActorNr)
    {
        // 이동 비활성화
        EditPlayerController.Instance.DisableMovement();

        BattleSystem.Instance.RegisterKill(killerActorNr, photonView.OwnerActorNr);
    }

    public void OnInLobby() => isInLobby = true;
    public void OnOutLobby() => isInLobby = false;
}
