using UnityEngine;

public class CreateRoomManager : MonoBehaviour
{
    [SerializeField] ButtonSound buttonSound;

    private void Awake()
    {
        buttonSound.RegisterButtonSounds();
    }

    public void OnCancelButtonClick()
    {
        gameObject.SetActive(false);
    }
}
