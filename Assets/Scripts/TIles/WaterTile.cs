using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaterTile : BasicTile
{
    public Ship PirateShip;

    private void MovePiratesOnTile()
    {
        foreach (var pirate in PirateShip.Pirates)
        {
            pirate.CurrentTile = this;
            Pirates.Add(pirate);
        }
        PlacePirateOnTile();
    }

    public override void LeavePirate(Pirate pirate)
    {
        if (PirateShip != null)
        {
            PirateShip.LeavePirate(pirate);
        }
        base.LeavePirate(pirate);
    }


    public override void EnterPirate(Pirate pirate)
    {
        if (PirateShip != null)
        {
            if (pirate.Ship.Id == this.PirateShip.Id)
            {
                PirateShip.EnterPirate(pirate);
                base.EnterPirate(pirate);
            }
            else
            {
                PirateShip.KillPirateForever(pirate);
            }
        }
        else
        {
            base.EnterPirate(pirate);
        }
    }

    public void EnterShip(Ship ship)
    {
        ship.CurrentTile = this;
        this.PirateShip = ship;

        ship.transform.position = this.transform.position + Vector3.down;
        MovePiratesOnTile();
    }

    public void LeaveShip()
    {
        this.PirateShip = null;
        LeaveAllPirates();
    }

    public void ShowAvailableForShipMoveCells()
    {
        var tilesList = this.GetPossibleTilesForShip();

        foreach (var tile in tilesList)
        {
            tile.GetComponentInChildren<MeshRenderer>().material.color = tile.EnterColor;
        }
    }

    public override void AddCoin(Coin coin)
    {
        if (PirateShip)
        {
            coin.CurrentTile.PopCoin();
            PirateShip.AddCoin(coin);
        }
        Destroy(coin.gameObject);
    }

    public void HideAvailableForShipMoveCells()
    {
        var tilesList = this.GetPossibleTilesForShip();

        foreach (var tile in tilesList)
        {
            tile.GetComponentInChildren<MeshRenderer>().material.color = tile.DefaultColor;
        }
    }

    public IEnumerable<Tile> GetPossibleTilesForShip()
    {
        var tiles = Map.SelectMany(g => g.ToArray()).Where(t => IsPossibleForShipMove(t));
        return tiles;
    }

    public bool IsPossibleForShipMove(Tile targetTile)
    {
        return (targetTile != this &&
        (targetTile is WaterTile) &&
        ((WaterTile)targetTile).PirateShip == null &&
        (Math.Abs(this.XPos - targetTile.XPos) < 2) &&
        (Math.Abs(this.YPos - targetTile.YPos) < 2));

    }

    public override bool IsPossibleForMove(Tile targetTile)
    {
        if (PirateShip != null)
        {
            return (targetTile != this &&
                !(targetTile is WaterTile) &&
                ((this.XPos == targetTile.XPos && Math.Abs(this.YPos - targetTile.YPos) < 2) ||
                (this.YPos == targetTile.YPos && Math.Abs(this.XPos - targetTile.XPos) < 2)));
        }
        else
        {
            return (targetTile != this &&
                (targetTile is WaterTile) &&
                (Math.Abs(this.XPos - targetTile.XPos) < 2) &&
                (Math.Abs(this.YPos - targetTile.YPos) < 2));
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (PirateShip && eventData.pointerDrag && eventData.pointerDrag.GetComponent<Pirate>())
        {
            PirateShip.Collider.enabled = false;
        }
        else if(PirateShip)
        {
            PirateShip.Collider.enabled = true;
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (PirateShip)
        {
            PirateShip.Collider.enabled = true;
        }
    }

    public override void PlacePirateOnTile()
    {
        if (!PirateShip)
        {
            base.PlacePirateOnTile();
        }
        else
        {
            int count = Pirates.Count;

            //Коэффициент для изменения размера пирата
            float factor = 0.75f;
            var localScale = new Vector3(factor, factor, factor);

            for (int i = 0; i < count; i++)
            {
                Pirates[i].transform.localScale = localScale;
                Pirates[i].transform.position = transform.position + Vector3.back + (i - 1) * Vector3.right;
            }
        }

    }
}
