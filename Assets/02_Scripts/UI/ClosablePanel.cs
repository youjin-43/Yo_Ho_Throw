using UnityEngine;

public class ClosablePanel : MonoBehaviour
{
    // 상위 패널 구분 용도. x버튼을 누르면 이 스크립트를 찾아 창을 닫게 할거임.
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
