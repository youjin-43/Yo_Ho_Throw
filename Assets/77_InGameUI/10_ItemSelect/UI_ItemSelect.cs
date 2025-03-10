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

        Button btn1 = transform.GetChild(2).GetComponent<Button>();
        Button btn2 = transform.GetChild(3).GetComponent<Button>();
        Button btn3 = transform.GetChild(4).GetComponent<Button>();

        btn1.onClick.AddListener(() => ItemSelected(_itemButton_1, 1));
        btn1.onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));

        btn2.onClick.AddListener(() => ItemSelected(_itemButton_2, 2));
        btn2.onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));

        btn3.onClick.AddListener(() => ItemSelected(_itemButton_3, 3));
        btn3.onClick.AddListener(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.UIClick));
    }
    #endregion





    #region FUNCTION
    public void ItemSelected(GameObject button, int index)
    {
        Debug.Log(index + "번 아이템 선택");

        InGameUIManager.Instance.ItemSelected(button.transform.GetChild(0).GetChild(0).GetComponent<Image>(), index);
    }
    #endregion
}
