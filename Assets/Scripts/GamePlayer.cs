using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    public int CoinsCount;
    public int PiratesCount = 3;

    public Text CoinText;
    public Text PirateText;
    public Text PlayerNameText;

    public void AddCoins()
    {
        CoinsCount++;
        CoinText.text = CoinsCount.ToString();
    }

    public void ChangePiratesCount(int value)
    {
        PiratesCount += value;
        PirateText.text = PiratesCount.ToString();
    }

    public void SetPlayerName(int id)
    {
        var player = PhotonNetwork.PlayerListOthers.FirstOrDefault(p => p.ActorNumber == id);
        PlayerNameText.text = player.NickName;
    }
}
