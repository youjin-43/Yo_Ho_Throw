using System;
using UnityEngine;

public class ManualManager : MonoBehaviour
{
    [SerializeField] GameObject[] Pages;
    private int currentPage;

    private void Start()
    {
        currentPage = 0; // 처음엔 항상 0페이지가 뜨도록
    }

    public void OnCloseButtonClicked()
    {
        gameObject.SetActive(false); // 메뉴얼 페이지 비활성화
    }

    public void OnRightButtonClicked()
    {
        Pages[currentPage].SetActive(false); // 현재페이지 비활성화

        if (currentPage < Pages.Length - 1) // 마지막 페이지가 아닐 경우
        {
            currentPage++;
        }
        else // 마지막 페이지일 경우
        {
            currentPage = 0; // 처음 페이지로 돌아감
        }

        Pages[currentPage].SetActive(true); // 다음페이지 활성화
    }

    public void OnLeftButtonClicked()
    {
        Pages[currentPage].SetActive(false); // 현재 페이지 비활성화

        if (currentPage > 0) // 첫 페이지가 아닐 경우
        {
            currentPage--;
        }
        else // 첫 페이지일 경우
        {
            currentPage = Pages.Length - 1; // 마지막 페이지로 설정
        }
        
        Pages[currentPage].SetActive(true); // 이전페이지 활성화
    }
}
