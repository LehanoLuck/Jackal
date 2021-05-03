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
    }

    public override void LeavePirate(Pirate pirate)
    {
        base.LeavePirate(pirate);
    }

    public void AddCoin(Coin coin)
    {
        this.Coins.Push(coin);
    }

    public Coin PopCoin()
    {
        return Coins.Pop();
    }

    public Coin PeekCoin()
    {
        return Coins.Peek();
    }

    public bool IsHaveCoins()
    {
        return Coins.Count > 0;
    }
}
