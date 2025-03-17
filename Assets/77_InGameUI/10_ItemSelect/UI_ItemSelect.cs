using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class UI_ItemSelect : UI_Base
{
    #region VARIABLES
    [SerializeField] Potion[] potions;
    private Animator animator;
    private GameObject _itemButton_1;
    private GameObject _itemButton_2;
    private GameObject _itemButton_3;
    private bool isFirstItemSelect = true;
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

        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_1, 1));
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_2, 2));
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ItemSelected(_itemButton_3, 3));
      
        animator = GetComponent<Animator>();
    }
    #endregion





    #region FUNCTION
    public void OnShowItemPanel()
    {
        GetComponent<PhotonView>().RPC("OnShowItemPanelRPC", RpcTarget.All);
    }
    [PunRPC]
    public void OnShowItemPanelRPC()
    {
        CursorController.Instance.CursorEnable();

        BattleUIController.Instance.SelectRandomPotionAfterDelay();

        animator.SetTrigger("OnShowItemPanel");
    }
    [SerializeField] Image[] potionImages;
    public void ItemSelected(GameObject button, int index)
    {
        Debug.Log(index + "번 아이템 선택");

        if (isFirstItemSelect)
        {
            BattleSystem.FirstItemSelect();

            animator.SetTrigger("OnHideItemPanel");

            isFirstItemSelect = false;

            BattleUIController.Instance.StopRandomPotionSelect();
        }
        ItemHandler.Instance.Equip(potions[index - 1]);
        InGameUIManager.Instance.ItemSelected(button.transform.GetChild(0).GetChild(0).GetComponent<Image>(), index);
    }
    public void ItemSelected(int index)
    {
        Debug.Log(index + "번 아이템 선택");

        if (isFirstItemSelect)
        {
            BattleSystem.FirstItemSelect();

            animator.SetTrigger("OnHideItemPanel");

            isFirstItemSelect = false;

            BattleUIController.Instance.StopRandomPotionSelect();
        }
        ItemHandler.Instance.Equip(potions[index - 1]);
        InGameUIManager.Instance.ItemSelected(potionImages[index-1], index);
    }
    #endregion
}
