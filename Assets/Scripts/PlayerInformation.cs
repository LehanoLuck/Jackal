using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public MapManager mapManager;
    public Text CoinValue;
    public bool IsMoveWithCoin = false;
    public byte CoinsCount = 0;

    public void Switch()
    {
        IsMoveWithCoin = !IsMoveWithCoin;
    }

    public void AddCoin()
    {
        CoinsCount++;
        CoinValue.text = CoinsCount.ToString();

        mapManager.CoinsLeft--;
        mapManager.Log.text = $"Coins left - {mapManager.CoinsLeft}";

        if (mapManager.CoinsLeft == 0)
        {
            RaiseEventManager.RaiseEndGameEvent(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

}
