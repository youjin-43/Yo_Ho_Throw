using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    protected string _name;
    public    bool   _isAlwaysVisible;

    public abstract void Init();
    public abstract void On();
    public abstract void Off();
    public abstract void ResetUI();

    public void ToggleUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
