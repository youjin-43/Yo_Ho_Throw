using UnityEngine;

public class EditPlayerState : MonoBehaviour, IDamagable
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

            if (hp == 0)
            {
                EditPlayerController.StopMove();


            }
        }
    }
    public void ReceiveDamage(int attackerActorNumber, int damage)
    {

    }
}
