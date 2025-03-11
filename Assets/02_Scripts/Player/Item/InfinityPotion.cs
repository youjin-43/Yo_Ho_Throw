using UnityEngine;

public class InfinityPotion : Potion
{
    PlayerController playerController = null;
    public override void Use()
    {
        if (playerController == null)
            playerController = PlayerSpawnManager.Instance.currPlayer.GetComponent<PlayerController>();

        playerController.GetInfinityKnifeBuff();
    }
}
