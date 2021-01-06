using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTile : GroundTile
{
    public Coin coinTemplate;

    public override void DoActionAfterOpenning()
    {
        base.DoActionAfterOpenning();
        Coin coin = Instantiate(coinTemplate, this.transform.parent);
        coin.transform.localScale = coin.transform.localScale * 0.6f;
        coin.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.1f, this.transform.position.z);
        this.Coins.Push(coin);
    }
}
