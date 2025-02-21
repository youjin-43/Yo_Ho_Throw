using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerScoreEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text killCountText;
    [SerializeField] TMP_Text deathCountText;
    [SerializeField] TMP_Text assistCountText;
    public void Init(Player player)
    {
        nickNameText.text = player.NickName;

        killCountText.text = "0";
        deathCountText.text = "0";
        assistCountText.text = "0";
    }
    public void SetKillCount(int killCount)
    {
        killCountText.text = killCount.ToString();
    }
    public void SetDeathCount(int deathCount)
    {
        deathCountText.text = deathCount.ToString();
    }
    public void SetAssistCount(int assistCount)
    {
        assistCountText.text = assistCount.ToString();
    }
}
