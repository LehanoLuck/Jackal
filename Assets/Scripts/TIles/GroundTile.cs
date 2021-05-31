using Assets.Scripts.TIles;
using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GroundTile : OpenedTile, ICoinInteractor, IPirateInteractor
{ 
    public Stack<Coin> Coins { get; set; } = new Stack<Coin>();

    public override void EnterPirate(Pirate pirate)
    {
        base.EnterPirate(pirate);
        if (pirate.IsMoveWithCoin())
        {
            pirate.SelfCoin.Move(this);
        }
        else
        {
            pirate.SelfCoin = this.PeekCoin();
        }
    }

    public override void AddCoin(Coin coin)
    {
        this.Coins.Push(coin);
    }

    public Coin PopCoin()
    {
        return Coins.Pop();
    }

    public Coin PeekCoin()
    {
        if (Coins.Count > 0)
        {
            return Coins.Peek();
        }
        else
        {
            return null;
        }
    }

    public bool IsHaveCoins()
    {
        return Coins.Count > 0;
    }
}
