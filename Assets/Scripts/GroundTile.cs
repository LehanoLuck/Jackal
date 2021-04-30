using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GroundTile : BasicTile, IPirateInteractor, ICoinInteractor
{ 

    public Stack<Coin> Coins { get; set; } = new Stack<Coin>();

    public override void EnterPirate(Pirate pirate)
    {
        bool isCanMoveYet;

        if (pirate.MovementSettings.IsAttack)
        {
            isCanMoveYet = this.TryAttack(pirate);
        }
        else
        {
            if (pirate.MovementSettings.IsMoveWithCoin)
                pirate.TakeCoinFromTile();
            isCanMoveYet = true;
        }

        if (isCanMoveYet)
        {
            this.Pirates.Add(pirate);
            SetCurrentPirateTile(pirate);
            PlacePirateOnTile();
        }
    }

    public void LeavePirate(Pirate pirate)
    {
        this.Pirates.Remove(pirate);
    }

    public void LeaveAllPirates()
    {
        this.Pirates.Clear();
    }

    protected virtual void SetCurrentPirateTile(Pirate pirate)
    {
        pirate.CurrentTile = this;

        if (pirate.MovementSettings.IsMoveWithCoin)
        {
            this.AddCoin(pirate.SelfCoin);
            pirate.SelfCoin.transform.position = this.transform.position;
        }

        pirate.SelfCoin = null;
    }

    public virtual bool TryAttack(Pirate pirate)
    {
        this.Attack(pirate);
        return true;
    }

    public virtual void Attack(Pirate pirate)
    {
        foreach (Pirate target in Pirates)
        {
            target.Die();
        }

        this.LeaveAllPirates();
    }

    public void PlacePirateOnTile()
    {
        int count = Pirates.Count;

        //Коэффициент для изменения размера пирата
        float factor = (float)(1 / Math.Pow(Math.Log(Math.E * count), 0.4));
        var localScale = new Vector3(factor, factor, factor);

        float radius = count == 1 ? 0 : this.transform.localScale.x * 0.85f;

        for (int i = 0; i < count; i++)
        {
            Pirates[i].transform.localScale = localScale;

            float value = i * 2f * Mathf.PI / count;

            float x = this.transform.position.x + radius * Mathf.Sin(value);
            float z = this.transform.position.z + radius * Mathf.Cos(value);

            Pirates[i].transform.position = new Vector3(x, this.transform.position.y - 1.25f, z);
        }
    }

    public void AddCoin(Coin coin)
    {
        this.Coins.Push(coin);
    }

    public Coin PopCoin()
    {
        return Coins.Pop();
    }

    public bool IsHaveCoins()
    {
        return Coins.Count > 0;
    }

    public Pirate GetPirate()
    {
        return this.isHavePirates ? Pirates[0] : null;
    }

    public override void DoActionAfterOpenning()
    {
    }
}
