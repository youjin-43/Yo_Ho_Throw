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
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public static void FadeIn()
    {
        Instance.photonView.RPC("FadeInRPC", RpcTarget.All);
    }
    public static void FadeOut()
    {
        Instance.photonView.RPC("FadeOutRPC", RpcTarget.All);
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

    Action fadeAction = null;
    public void FadeAction()
    {
        fadeAction?.Invoke();

        fadeAction = null;
    }
}