using System.Collections;
using UnityEngine;

public class ExplosionPotion : Potion
{
    PlayerController playerController = null;

    public override void Use()
    {
        if (playerController == null)
            playerController = PlayerSpawnManager.Instance.currPlayer.GetComponent<PlayerController>();

        playerController.GetExplosionBuff();
    }
}