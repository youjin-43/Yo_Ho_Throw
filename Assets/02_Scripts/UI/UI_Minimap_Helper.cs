using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UI_Minimap_Helper : MonoBehaviourPunCallbacks
{
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        InGameUIManager.Instance.Minimap.RemovePlayerTransform(otherPlayer.ActorNumber);
    }
}
