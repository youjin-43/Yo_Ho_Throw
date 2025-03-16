using Photon.Pun;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ScreenTransition : MonoBehaviourPun
{
    public static ScreenTransition Instance { get; private set; } = null;

    [SerializeField] Animator alphaMaskController;
    private void Awake()
    {
        Instance = this;
    }
    public static void FadeIn()
    {
        Instance.photonView.RPC("FadeInRPC", RpcTarget.All);
    }
    public static void FadeOut()
    {
        Instance.photonView.RPC("FadeOutRPC", RpcTarget.All);
    }
    public void FadeActionSetting(Action action)
    {
        fadeAction = action;
    }
    [PunRPC]
    public void FadeInRPC()
    {
        alphaMaskController.SetTrigger("FadeIn");
    }
    [PunRPC]
    public void FadeOutRPC()
    {
        alphaMaskController.SetTrigger("FadeOut");
    }

    Action fadeAction = null;
    public void FadeAction()
    {
        fadeAction?.Invoke();

        fadeAction = null;
    }
}