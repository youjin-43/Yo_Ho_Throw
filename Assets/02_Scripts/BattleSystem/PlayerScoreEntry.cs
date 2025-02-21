using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text killCountText;
    [SerializeField] TMP_Text deathCountText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] Image iconImage;
    public void Init(Player player)
    {
        nickNameText.text = player.NickName;

        ResetPlayerScoreEntry();
    }
    public void ResetPlayerScoreEntry()
    {
        killCountText.text = "0";
        deathCountText.text = "0";
        scoreText.text = "0";
    }
    public void SetKillCount(int killCount)
    {
        killCountText.text = killCount.ToString();
    }
    public void SetDeathCount(int deathCount)
    {
        deathCountText.text = deathCount.ToString();
    }
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public void SetIconImage(Sprite sprite)
    {
        iconImage.gameObject.SetActive(true);

        iconImage.sprite = sprite;
    }
}
