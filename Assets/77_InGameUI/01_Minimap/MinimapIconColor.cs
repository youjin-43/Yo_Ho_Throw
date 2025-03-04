using UnityEngine;

[CreateAssetMenu(fileName = "MinimapIconColor", menuName = "UI/MinimapIconColor")]
public class MinimapIconColor : ScriptableObject
{
    public Color my_Player_Color;
    public Color other_Player_Color;

    public Color bounty_Hunter_Color;
    public Color revenge_Target_Color;

    public Color treasure_Box_Color;

    public Sprite my_Player_Sprite;
    public Sprite other_Player_Sprite;

    public Sprite bounty_Hunter_Sprite;
    public Sprite revenge_Target_Sprite;

    public Sprite treasure_Box_Sprite;
}
