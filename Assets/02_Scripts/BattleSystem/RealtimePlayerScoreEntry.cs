using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RealtimePlayerScoreEntry : MonoBehaviour
{
    public TMP_Text nickNameText;
    public TMP_Text scoreText;
    public TMP_Text rankText;

    public void Init(Player player)
    {
        nickNameText.text = player.NickName;
        scoreText.text = "0";
        rankText.text = "1";
    }
    public void SetRank(int rank)
    {
        rankText.text = rank.ToString();
    }
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public void SetNickName(string nickName)
    {
        nickNameText.text = nickName.ToString();
    }
}
