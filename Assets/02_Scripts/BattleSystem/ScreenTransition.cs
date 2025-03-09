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
    public static void FadeIn()
    {
        instance.photonView.RPC("FadeInRPC", RpcTarget.All);
    }
    public static void FadeOut()
    {
        instance.photonView.RPC("FadeOutRPC", RpcTarget.All);
    }
    [PunRPC]
    public void FadeInRPC()
    {
        Debug.Log("FadeIn");
        alphaMaskController.SetTrigger("FadeIn");
    }
    [PunRPC]
    public void FadeOutRPC()
    {
        Debug.Log("FadeOut");
        alphaMaskController.SetTrigger("FadeOut");
    }
}