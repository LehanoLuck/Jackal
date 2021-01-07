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

    public ShipTile Ship;

    public Player SelfPlayer;

    private bool isCanMove;
    private bool isPlacebleTile;
    public bool isAttack;
    public bool isMoveWithCoin;

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<MeshCollider>();
    }

    #region DragEvents

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

    #endregion

    #region MovableOptions

    private void SetMovableOptions(PointerEventData eventData)
    {
        TargetTile = GetTargetTile();

        if (TargetTile)
        {
            this.CanPlaceOnTile(TargetTile);
        }
        else
        {
            isCanMove = false;
            isPlacebleTile = false;
        }

        BaseTile GetTargetTile()
        {
            var point = eventData.pointerEnter;
            if (point)
            {
                if (point.GetComponent<BaseTile>())
                {
                    isCanMove = true;
                    return eventData.pointerEnter.GetComponent<BaseTile>();
                }
                else if (point.GetComponent<Pirate>())
                {
                    isCanMove = true;
                    return eventData.pointerEnter.GetComponent<Pirate>().CurrentTile;
                }
            }
            return null;
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
        isAttack = targetTile.isHavePirates ? IsFriendlyPirates() : false;

        bool IsFriendlyPirates()
        {
            var pirate = targetTile.GetPirate();

            if (pirate.Ship != this.Ship)
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
            CurrentTile.LeavePirate(this);
            TargetTile.EnterPirate(this);
        }
        else
        {
            transform.position = this.StartPosition;
        }
    }

    public void TakeCoinFromTile()
    {
        this.SelfCoin = CurrentTile.PopCoin();
    }

    public void Die()
    {
        //Заглушка чтобы не уносить с собой при смерти монетки
        this.isMoveWithCoin = false;
        this.isAttack = false;
        this.Ship.EnterPirate(this);
    }

    #endregion
}
