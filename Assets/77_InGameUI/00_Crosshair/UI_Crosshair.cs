using TMPro;
using UnityEngine;

public class UI_Crosshair : UI_Base
{
    #region VARIABLES
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





    #region BEHAVIOURS
    void Awake()
    {
        
    }
    #endregion





    #region FUNCTION
    
    #endregion
}
