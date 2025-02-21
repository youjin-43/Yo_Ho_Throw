using UnityEngine;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] Transform PlayerTransform;
    [SerializeField] Transform MinimapCamera;

    void LateUpdate()
    {
        MinimapCamera.position = new Vector3(PlayerTransform.position.x, 100f, PlayerTransform.position.z);
        MinimapCamera.rotation = Quaternion.Euler(90f, PlayerTransform.localEulerAngles.y, 0f);
    }
}
