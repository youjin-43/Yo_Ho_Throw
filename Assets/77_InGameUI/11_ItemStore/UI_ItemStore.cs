using UnityEngine;
using UnityEngine.UI;

public class UI_ItemStore : UI_Base
{
    #region VARIABLES
    private GameObject _itemButton_1;
    private GameObject _itemButton_2;
    private GameObject _itemButton_3;

    private GameObject _disablePanel;
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

        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ItemPurchace(_itemButton_1, 1));
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ItemPurchace(_itemButton_2, 2));
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ItemPurchace(_itemButton_3, 3));
    

        _disablePanel = transform.GetChild(5).gameObject;

        _disablePanel.SetActive(false);
    }
    #endregion





    #region FUNCTION

    // 이 버튼을 누르면 아이템이 구매가 
    public void ItemPurchace(GameObject button, int index)
    {
        Debug.Log(index + "번 아이템 구매");

        InGameUIManager.Instance.ItemPurchase(button.transform.GetChild(0).GetChild(0).GetComponent<Image>(), index);

        PurchaceDeActivation();
    }

    public void PurchaceActivation(int coin)
    {
        if (coin >= 5)
        {
            _disablePanel.gameObject.SetActive(false);
        }
    }

    public void PurchaceDeActivation()
    {
        _disablePanel.gameObject.SetActive(true);
    }
    #endregion
}
