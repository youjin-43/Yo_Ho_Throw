using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ScreenTransition : MonoBehaviourPun
{
    static ScreenTransition instance = null;

    [SerializeField] Animator alphaMaskController;
    private void Awake()
    {
        instance = this;
    }
    public static void FadeInRPC()
    {
        instance.photonView.RPC("FadeIn", RpcTarget.All);
    }
    public static void FadeOutRPC()
    {
        instance.photonView.RPC("FadeOut", RpcTarget.All);
    }
    [PunRPC]
    public void FadeIn()
    {
        alphaMaskController.SetTrigger("FadeIn");
    }
    [PunRPC]
    public void FadeOut()
    {
        alphaMaskController.SetTrigger("FadeOut");
    }
}