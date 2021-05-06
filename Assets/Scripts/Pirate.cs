using Assets.Scripts;
using Assets.Scripts.TIles.Interfaces;
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
    public BasicTile CurrentTile { get; set; }
    private Tile TargetTile { get; set; }

    private PhotonView photonView;

    public ShipTile Ship;

    public GamePlayer SelfPlayer;

    public StepByStepSystem StepSystem;

    public TrajectoryMovement trajectoryMovement;

    private bool isMyTurn => (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"];

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
        if (!photonView.IsMine || !isMyTurn)
            return;

        var groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);
            //worldPosition.y += 1.5f;

            //transform.position = worldPosition;

            trajectoryMovement.ShowTrajectory(worldPosition);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = false;
        StartPosition = this.transform.position;
        CurrentTile.ShowAvailableForMoveCells();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = true;
        CurrentTile.HideAvailableForMoveCells();
        var isCanMove = IsCanMoveOnTile(eventData);

        if (isCanMove)
        {

            var pirateMovementData = new PirateMovementData
            {
                Id = this.Id,
                ShipId = this.Ship.Id,
                XPos = TargetTile.XPos,
                YPos = TargetTile.YPos,
                IsMoveWithCoin = IsMoveWithCoin()
            };

            StepByStepSystem.StartNextTurn();
            RaiseEventManager.RaiseMovePirateEvent(pirateMovementData);
            MoveOnTile();
        }
        else
        {
            transform.position = this.StartPosition;
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
                else if (point.GetComponent<Pirate>())
                {
                    return eventData.pointerEnter.GetComponent<Pirate>().CurrentTile;
                }
            }
            return null;
        }
    }

    public bool IsMoveWithCoin()
    {
        return SelfPlayer.isMoveWithItem && SelfCoin != null;
    }

    //private MovementSettings SetMovableOptions(Tile targetTile)
    //{
    //    bool isMoveWithCoin = SetCoinWithMoveMode(isAttack);
    //    return new MovementSettings(isMoveWithCoin);
    //}

    //private bool SetAttackMode(Tile targetTile)
    //{
    //    if (targetTile is BasicTile)
    //    {
    //        return ((BasicTile)targetTile).isHavePirates && IsFriendlyPirates();
    //    }
    //    return false;


    //    bool IsFriendlyPirates()
    //    {
    //        var pirate = ((BasicTile)targetTile).GetPirate();

    //        if (pirate.Ship != this.Ship)
    //            return true;
    //        else
    //            return false;
    //    }
    //}

    //private bool SetCoinWithMoveMode(bool isAttack)
    //{
    //    if(CurrentTile is GroundTile)
    //    {
    //        return ((GroundTile)CurrentTile).IsHaveCoins && SelfPlayer.isMoveWithItem && !isAttack;
    //    }
    //    return false;

    //}
    #endregion

    #region MovableActions

    public void MoveOnTile(PirateMovementData data, BasicTile targetTile)
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
        //Заглушка чтобы не уносить с собой при смерти монетки
        //Создаю новый экземпляр класса, чтобы нормально отправить событие в RaiseMovePirateEvent
        //И всю эту кашу нужно разгрести!
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

    public void DropCoin()
    {
        SelfCoin = null;
    }
}

//public class MovementSettings
//{
//    public MovementSettings(bool isAttack, bool isMovewithCoin)
//    {
//        this.IsAttack = isAttack;
//        this.IsMoveWithCoin = isMovewithCoin;
//    }

//    public MovementSettings()
//    {
//        IsAttack = false;
//        IsMoveWithCoin = false;
//    }

//    public bool IsAttack { get; set; }
//    public bool IsMoveWithCoin { get; set; }

//    public static object Deserialize(byte[] data)
//    {
//        var result = new MovementSettings();

//        result.IsAttack = Convert.ToBoolean(data[0]);
//        result.IsMoveWithCoin = Convert.ToBoolean(data[1]);
//        return result;
//    }

//    public static byte[] Serialize(object data)
//    {
//        var settings = (MovementSettings)data;
//        return
//            new byte[]
//            {
//                Convert.ToByte(settings.IsAttack),
//                Convert.ToByte(settings.IsMoveWithCoin),
//            };
//    }
//}
