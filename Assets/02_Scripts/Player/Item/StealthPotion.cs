using Photon.Pun;
using UnityEngine;

public class StealthPotion : Potion
{
    PhotonView playerPhotonView = null;

    public override void Use()
    {
        if (playerPhotonView == null)
            playerPhotonView = PlayerSpawnManager.Instance.currPlayer.GetComponent<PhotonView>();

        playerPhotonView.RPC("StealthSetting", RpcTarget.All);
    }
}
