using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Start()
    {
        DefaultColor = GetComponentInChildren<MeshRenderer>().material.color;
        EnterColor = DefaultColor += new Color(0, 100, 0, 100);
        ClosedColor = DefaultColor += new Color(100, 0, 0, 100);
    }

    protected Color DefaultColor;
    protected Color EnterColor;
    protected Color ClosedColor;

    public Stack<Coin> Coins { get; set; } = new Stack<Coin>();

    public List<Pirate> Pirates { get; set; } = new List<Pirate>();

    public BaseTile[][] Map { get; set; }

    public byte XPos { get; set; }

    public byte YPos { get; set; }

    public int MaxPirateSize = 5;

    internal Vector3 fixedPosition;
    protected Vector3 updatePosition;

    public bool isHaveCoins => Coins.Count > 0;

    public bool isHavePirates => Pirates.Count > 0;

    public void SetTransformPosition(Vector3 position)
    {
        this.transform.position = position;
        this.fixedPosition = position;
        this.updatePosition = new Vector3(position.x, position.y + 0.25f, position.z);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.position = updatePosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.position = fixedPosition;
    }

    #region PirateActions

    public virtual void EnterPirate(Pirate pirate)
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

        if(isCanMoveYet)
        {
            this.Pirates.Add(pirate);
            SetCurrentPirateTile(pirate);
            PlacePirateOnTile();
        }
    }

    public void LeaveAllPirates()
    {
        this.Pirates.Clear();
    }

    public void LeavePirate(Pirate pirate)
    {
        this.Pirates.Remove(pirate);

        PlacePirateOnTile();
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

    protected virtual void SetCurrentPirateTile(Pirate pirate)
    {
        pirate.CurrentTile = this;

        if(pirate.MovementSettings.IsMoveWithCoin)
        {
            this.AddCoin(pirate.SelfCoin);
            pirate.SelfCoin.transform.position = this.transform.position;
        }

        pirate.SelfCoin = null;
    }

    public Pirate GetPirate()
    {
        return this.isHavePirates ? Pirates[0] : null;
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
    #endregion

    public virtual void AddCoin(Coin coin)
    {
        this.Coins.Push(coin);
    }

    public virtual Coin PopCoin()
    {
        return Coins.Pop();
    }

    public void ShowAvailableForMoveCells()
    {
        var tilesList = this.GetPossibleTiles();

        foreach (var tile in tilesList)
        {
            tile.GetComponentInChildren<MeshRenderer>().material.color = tile.EnterColor;
        }
    }

    protected virtual IEnumerable<BaseTile> GetPossibleTiles()
    {
        var tiles = Map.SelectMany(g => g.ToArray()).Where(t=>t.IsCanMoveOnThisTileFromCurrentTile(this)).ToList();
        return tiles;
    }

    public bool IsCanMoveOnThisTileFromCurrentTile(BaseTile currentTile)
    {
        return (currentTile != this &&
                (Math.Abs(this.XPos - currentTile.XPos) < 2) &&
                (Math.Abs(this.YPos - currentTile.YPos) < 2) &&
                !(this is WaterTile) &&
                (this.Pirates.Count < this.MaxPirateSize));
    }
}
