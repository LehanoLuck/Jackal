using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


public class Pirate : SelectableObject, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject MoveCoinsButtonTemplate;

    private Camera GameCamera;

    public MeshCollider Collider;

    private Vector3 StartPosition;

    public BaseTile CurrentTile { get; set; }

    private BaseTile tempTile;

    public ShipTile ship;

    public Coin SelfCoin;

    public Player SelfPlayer;

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<MeshCollider>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);
            worldPosition.y += 1.5f;

            transform.position = worldPosition;

            if (IsMoveWithItem())
            {
                SelfCoin.transform.position = this.transform.position;
            }
        }

        //Получение временной ячейки, костыль чтобы запомнить ячейку при отпускании пирата
        if(eventData.pointerEnter && eventData.pointerEnter.GetComponent<BaseTile>())
        {
            tempTile = eventData.pointerEnter.GetComponent<BaseTile>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Collider.enabled = false;
        StartPosition = this.transform.position;

        if (IsMoveWithItem())
        {
            SelfCoin = CurrentTile.Coins[0];
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider.enabled = true;

        if (eventData.pointerEnter)
        {
            if (TryMoveOnTile(tempTile))
            {
                if (TryAttack(tempTile))
                {
                    if (IsMoveWithItem())
                        ReplaceCoin(SelfCoin);

                    CurrentTile.LeavePirate(this);
                    tempTile.EnterPirate(this);
                }
            }
            else
            {
                this.transform.position = StartPosition;
            }
        }
        else
        {
            this.transform.position = StartPosition;
        }

        SelfCoin = null;
    }

    private void ReplaceCoin(Coin coin)
    {
        CurrentTile.Coins.Remove(coin);
        tempTile.AddCoin(coin);
        coin.transform.position = tempTile.transform.position;
    }

    private bool TryMoveOnTile(BaseTile tile)
    {
        return (CurrentTile != tile &&
            (Math.Abs(tempTile.HorizontalIndex - CurrentTile.HorizontalIndex) < 2) && 
            (Math.Abs(tempTile.VerticalIndex - CurrentTile.VerticalIndex) < 2) && 
            !(tile is WaterTile) &&
            (tempTile.Pirates.Count < tempTile.maxSize));
    }

    public bool TryAttack(BaseTile tile)
    {
        if(tile is ShipTile)
        {
            CurrentTile.LeavePirate(this);
            Die();
            return false;
        }
        else if (tile.Pirates.Count != 0)
        {
            var pirate = tile.Pirates[0];

            if(pirate.ship != this.ship)
            {
                this.Attack(tile);
            }
        }
        return true;
    }

    private void Attack(BaseTile tile)
    {
        foreach(Pirate pirate in tile.Pirates)
        {
            pirate.Die();
        }
        tile.LeaveAllPirates();
    }

    public void Die()
    {
        this.ship.EnterPirate(this);
    }

    public bool IsMoveWithItem()
    {
        return CurrentTile.isHaveCoins && SelfPlayer.isMoveWithItem;
    }
}
