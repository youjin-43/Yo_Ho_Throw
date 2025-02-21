using UnityEngine;
using TMPro;
public class TitleUIManager : MonoBehaviour
{
    public TextMeshProUGUI userID;

    // Connet 버튼에 연결 
    public void SetUserID()
    {
        GameManager.Instance.UserId = userID.text;
        Debug.Log("유저 아이디 셋팅 : " + GameManager.Instance.UserId);
    }

    // TODO : 이제 안씀 
}
