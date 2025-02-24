using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    static ScreenTransition instance = null;

    [SerializeField] Animator alphaMaskController;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void FadeIn() => instance.alphaMaskController.SetTrigger("FadeIn");
    public static void FadeOut() => instance.alphaMaskController.SetTrigger("FadeOut");
}
