using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonImage : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite pressedButtonImg; // 버튼을 눌렀을 때 바뀌는 이미지
    [SerializeField] TMP_Text buttonText; // 버튼 텍스트


    public void OnButtonClicked()
    {
        buttonImage.sprite = pressedButtonImg;
        buttonText.color = Color.gray;
        buttonText.rectTransform.anchoredPosition = new Vector2 (0, -5);
    }
}
