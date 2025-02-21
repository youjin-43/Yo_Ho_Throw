using UnityEngine;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] Transform PlayerTransform;
    [SerializeField] Transform MinimapCamera;

    private Transform _minimapFrame;

    void Awake()
    {
        _minimapFrame = transform.GetChild(0);
    }

    void LateUpdate()
    {
        MinimapCamera.position = new Vector3(PlayerTransform.position.x, 100f, PlayerTransform.position.z);
        MinimapCamera.rotation = Quaternion.Euler(90f, PlayerTransform.localEulerAngles.y, 0f);
        _minimapFrame.rotation = Quaternion.Euler(0f, 0f, PlayerTransform.localEulerAngles.y);
    }

    public void ResetUI()
    {

    }
}
