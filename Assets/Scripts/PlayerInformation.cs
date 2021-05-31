using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
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
    }
}
