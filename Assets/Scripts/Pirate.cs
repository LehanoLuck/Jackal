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

    public Coin SelfCoin { get; set; }
    public BaseTile CurrentTile { get; set; }
    private BaseTile TargetTile { get; set; }

    public ShipTile ship;

    public Player SelfPlayer;

    private bool isCanMove;
    private bool isPlacebleTile;
    private bool isAttack;
    public bool isMoveWithCoin;

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
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Collider.enabled = false;
        StartPosition = this.transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider.enabled = true;

        this.SetMovableOptions(eventData);

        this.TryMoveOnTile();
    }

    #region MovableOptions

    private void SetMovableOptions(PointerEventData eventData)
    {
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<BaseTile>())
        {
            TargetTile = eventData.pointerEnter.GetComponent<BaseTile>();

            isCanMove = true;
            this.CanPlaceOnTile(TargetTile);
        }
        else
        {
            TargetTile = null;
            isCanMove = false;
            isPlacebleTile = false;
        }
    }

    private void CanPlaceOnTile(BaseTile targetTile)
    {
        isPlacebleTile = (CurrentTile != targetTile &&
            (Math.Abs(TargetTile.HorizontalIndex - CurrentTile.HorizontalIndex) < 2) &&
            (Math.Abs(TargetTile.VerticalIndex - CurrentTile.VerticalIndex) < 2) &&
            !(targetTile is WaterTile) &&
            (TargetTile.Pirates.Count < TargetTile.maxSize));

        if (isPlacebleTile)
        {
            SetAttackMode(targetTile);
        }
        else
        {
            isAttack = false;
        }
    }

    private void SetAttackMode(BaseTile targetTile)
    {
        isAttack = targetTile.Pirates.Count > 0 ? IsFriendlyPirates() : false;

        bool IsFriendlyPirates()
        {
            var pirate = targetTile.Pirates[0];

            if (pirate.ship != this.ship)
                return true;
            else
                return false;
        }

        if (isAttack)
        {
            isMoveWithCoin = false;
        }
        else
        {
            SetCoinWithMoveMode();
        }
    }

    private void SetCoinWithMoveMode()
    {
        isMoveWithCoin = CurrentTile.isHaveCoins && SelfPlayer.isMoveWithItem;
    }
    #endregion

    #region MovableActions

    private void TryMoveOnTile()
    {
        if (isCanMove && isPlacebleTile)
        {
            if (isAttack)
            {
                TryAttack();
            }
            else
            {
                if (isMoveWithCoin)
                    TakeCoinFromTile();
            }

            CurrentTile.LeavePirate(this);
            TargetTile.EnterPirate(this);
        }
        else
        {
            transform.position = this.StartPosition;
        }
    }

    private void TryAttack()
    {
        if (TargetTile is ShipTile && this.ship != TargetTile)
        {
            CurrentTile.LeavePirate(this);
            Die();
        }
        else
        {
            this.Attack(TargetTile);
        }
    }

    private void Attack(BaseTile tile)
    {
        foreach (Pirate pirate in tile.Pirates)
        {
            pirate.Die();
        }
        tile.LeaveAllPirates();
    }

    private void TakeCoinFromTile()
    {
        this.SelfCoin = CurrentTile.PopCoin();
    }

    public void Die()
    {
        this.ship.EnterPirate(this);
    }

    #endregion
}
