using Assets.Scripts;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


public class Pirate : SelectableObject, IDragHandler, IBeginDragHandler, IEndDragHandler, IPunInstantiateMagicCallback
{
    public byte Id { get; set; }

    public GameObject MoveCoinsButtonTemplate;

    private Camera GameCamera;

    public MeshCollider Collider;

    private Vector3 StartPosition;

    public Coin SelfCoin { get; set; }
    public BaseTile CurrentTile { get; set; }
    private BaseTile TargetTile { get; set; }

    private PhotonView photonView;

    public ShipTile Ship;

    public Player SelfPlayer;

    public MovementSettings MovementSettings = new MovementSettings();

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<MeshCollider>();

        photonView = GetComponent<PhotonView>();
    }

    public void SetShip(ShipTile ship)
    {
        this.Ship = ship;
        ship.ShipPirates.Add(this);
    }

    #region DragEvents

    public void OnDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine)
            return;

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
        if (!photonView.IsMine)
            return;

        Collider.enabled = false;
        StartPosition = this.transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine)
            return;

        Collider.enabled = true;

        var isCanMove = IsCanMoveOnTile(eventData);

        if (isCanMove)
        {
             MovementSettings = this.SetMovableOptions(TargetTile);

            var pirateMovementData = new PirateMovementData
            {
                Id = this.Id,
                ShipId = this.Ship.Id,
                XPos = TargetTile.HorizontalIndex,
                YPos = TargetTile.VerticalIndex,
                Settings = MovementSettings
            };

            MoveOnTile();
            RaiseEventManager.RaiseMovePirateEvent(pirateMovementData);
        }
        else
        {
            transform.position = this.StartPosition;
        }
    }

    #endregion

    #region MovableOptions

    private bool IsCanMoveOnTile(PointerEventData eventData)
    {
        TargetTile = GetTargetTile();

        if(TargetTile)
        {
            return IsCanMoveOnTile();
        }
        else
        {
            return false;
        }

        BaseTile GetTargetTile()
        {
            var point = eventData.pointerEnter;
            if (point)
            {
                if (point.GetComponent<BaseTile>())
                {
                    return eventData.pointerEnter.GetComponent<BaseTile>();
                }
                else if (point.GetComponent<Pirate>())
                {
                    return eventData.pointerEnter.GetComponent<Pirate>().CurrentTile;
                }
            }
            return null;
        }

        bool IsCanMoveOnTile()
        {
            return (CurrentTile != TargetTile &&
            (Math.Abs(TargetTile.HorizontalIndex - CurrentTile.HorizontalIndex) < 2) &&
            (Math.Abs(TargetTile.VerticalIndex - CurrentTile.VerticalIndex) < 2) &&
            !(TargetTile is WaterTile) &&
            (TargetTile.Pirates.Count < TargetTile.maxSize));
        }
    }

    private MovementSettings SetMovableOptions(BaseTile targetTile)
    {
        bool isAttack = SetAttackMode(targetTile);
        bool isMoveWithCoin = SetCoinWithMoveMode(isAttack);
        return new MovementSettings(isAttack,isMoveWithCoin);
    }

    private bool  SetAttackMode(BaseTile targetTile)
    {
        return targetTile.isHavePirates ? IsFriendlyPirates() : false;

        bool IsFriendlyPirates()
        {
            var pirate = targetTile.GetPirate();

            if (pirate.Ship != this.Ship)
                return true;
            else
                return false;
        }
    }

    private bool SetCoinWithMoveMode(bool isAttack)
    {
        return CurrentTile.isHaveCoins && SelfPlayer.isMoveWithItem && !isAttack;
    }
    #endregion

    #region MovableActions

    public void MoveOnTile(PirateMovementData data, BaseTile targetTile)
    {
        this.MovementSettings = data.Settings;
        TargetTile = targetTile;

        MoveOnTile();
    }

    public void MoveOnTile()
    {
        CurrentTile.LeavePirate(this);
        TargetTile.EnterPirate(this);
    }

    public void TakeCoinFromTile()
    {
        this.SelfCoin = CurrentTile.PopCoin();
    }

    public void Die()
    {
        //Заглушка чтобы не уносить с собой при смерти монетки
        this.MovementSettings.IsMoveWithCoin = false;
        this.MovementSettings.IsAttack = false;
        this.Ship.EnterPirate(this);
    }

    #endregion

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        this.Id = (byte)data[0];
        var mapManager = FindObjectOfType<MapManager>();
        var ship = mapManager.ShipTiles[(byte)data[1]];
        SetShip(ship);
        ship.EnterPirate(this);
    }
}

public class MovementSettings
{
    public MovementSettings(bool isAttack, bool isMovewithCoin)
    {
        this.IsAttack = isAttack;
        this.IsMoveWithCoin = isMovewithCoin;
    }

    public MovementSettings()
    {
        IsAttack = false;
        IsMoveWithCoin = false;
    }

    public bool IsAttack { get; set; }
    public bool IsMoveWithCoin { get; set; }

    public static object Deserialize(byte[] data)
    {
        var result = new MovementSettings();

        result.IsAttack = Convert.ToBoolean(data[0]);
        result.IsMoveWithCoin = Convert.ToBoolean(data[1]);
        return result;
    }

    public static byte[] Serialize(object data)
    {
        var settings = (MovementSettings)data;
        return
            new byte[]
            {
                Convert.ToByte(settings.IsAttack),
                Convert.ToByte(settings.IsMoveWithCoin),
            };
    }
}
