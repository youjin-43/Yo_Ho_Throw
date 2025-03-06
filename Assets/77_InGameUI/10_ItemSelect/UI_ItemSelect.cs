using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSelect : UI_Base
{
    #region VARIABLES
    private GameObject _itemButton_1;
    private GameObject _itemButton_2;
    private GameObject _itemButton_3;
    #endregion





    #region OVERRIDE
    public override void Init()
    {
        _name = name;
    }

    public override void On()
    {
        gameObject.SetActive(true);
    }

    public override void Off()
    {
        gameObject.SetActive(false);
    }

    public override void ResetUI()
    {
    }
    #endregion





    #region MONOBEHAVIOUR
    void Awake()
    {
        _itemButton_1 = transform.GetChild(2).gameObject;
        _itemButton_2 = transform.GetChild(3).gameObject;
        _itemButton_3 = transform.GetChild(4).gameObject;

        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_1));
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_2));
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_3));

        Cursor.lockState = CursorLockMode.None;
    }
    #endregion





    #region FUNCTION
    /// <summary>
    /// 플레이어가 아이템을 선택했는지 판별하는 함수입니다.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public void ItemSelected(GameObject button)
    {
        InGameUIManager.Instance.ItemSelected(button.transform.GetChild(0).GetChild(0).GetComponent<Image>());
    }
    #endregion
}
