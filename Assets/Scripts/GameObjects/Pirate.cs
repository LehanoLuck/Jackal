using Assets.Scripts;
using Assets.Scripts.TIles.Interfaces;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


public class Pirate : SelectableObject, IDragHandler, IBeginDragHandler, IEndDragHandler, IPunInstantiateMagicCallback
{
    public byte Id { get; set; }

    private Camera GameCamera;
    public BoxCollider Collider;
    private PlayerInformation pirateMovableOptions;
    public Coin SelfCoin { get; set; }
    public BasicTile CurrentTile { get; set; }
    public Tile TargetTile { get; set; }

    private PhotonView photonView;

    public Ship Ship;

    public TrajectoryMovement trajectoryMovement;

    private bool isMyTurn => (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"];

    public bool isMoveWithCoin = false;

    public bool isActive = true;

    void Start()
    {
        GameCamera = Camera.main;

        photonView = GetComponent<PhotonView>();
        Collider = GetComponent<BoxCollider>();
        pirateMovableOptions = FindObjectOfType<PlayerInformation>();
    }

    #region DragEvents

    public void OnDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn || !isActive)
            return;

        var groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);

            trajectoryMovement.ShowTrajectory(worldPosition);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn || !isActive)
            return;

        CurrentTile.ShowAvailableForMoveCells();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn || !isActive)
            return;

        isMoveWithCoin = pirateMovableOptions.IsMoveWithCoin;
        CurrentTile.HideAvailableForMoveCells();
        var isCanMove = IsCanMoveOnTile(eventData);

        if (isCanMove)
        {
            Deactivate();
            var pirateMovementData = new PirateMovementData
            {
                Id = this.Id,
                ShipId = this.Ship.Id,
                XPos = TargetTile.XPos,
                YPos = TargetTile.YPos,
                IsMoveWithCoin = IsMoveWithCoin()
            };

            RaiseEventManager.RaiseMovePirateEvent(pirateMovementData);
            MoveOnTile();

            if (!isActive)
            {
                StepByStepSystem.StartNextTurn();
                Activate();
            }
        }
        this.trajectoryMovement.HideTrajectory();
    }

    #endregion

    #region MovableOptions

    private bool IsCanMoveOnTile(PointerEventData eventData)
    {
        TargetTile = GetTargetTile();

        if(TargetTile)
        {
            return CurrentTile.IsPossibleForMove(TargetTile);
        }
        else
        {
            return false;
        }

        Tile GetTargetTile()
        {
            var point = eventData.pointerEnter;
            if (point)
            {
                if (point.GetComponent<Tile>())
                {
                    return eventData.pointerEnter.GetComponent<Tile>();
                }
            }
            return null;
        }
    }

    public bool IsMoveWithCoin()
    {
        return isMoveWithCoin && SelfCoin != null;
    }

    #endregion

    #region MovableActions

    public void MoveOnTile(PirateMovementData data, Tile targetTile)
    {
        TargetTile = targetTile;

        MoveOnTile();
    }

    public void MoveOnTile()
    {
        CurrentTile.LeavePirate(this);
        TargetTile.EnterPirate(this);
    }

    public void Die()
    {
        DropCoin();
        this.Ship.CurrentTile.EnterPirate(this);
    }

    public void DieForever()
    {
        Destroy(this.gameObject);
    }

    #endregion

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        this.Id = (byte)data[0];
        var mapManager = FindObjectOfType<MapManager>();
        var ship = mapManager.ShipsDictionary[(byte)data[1]];
        Ship = ship;
        ship.CurrentTile.EnterPirate(this);
        ship.ShipPirates.Add(Id, this);
    }

    public void DropCoin()
    {
        SelfCoin = null;
    }

    public void Activate()
    {
        this.Ship.isActive = true;

        foreach (var pirate in this.Ship.ShipPirates.Values)
        {
            pirate.isActive = true;
        }
    }

    public void Deactivate()
    {
        this.Ship.isActive = false;

        foreach (var pirate in this.Ship.ShipPirates.Values)
        {
            pirate.isActive = false;
        }
    }
}