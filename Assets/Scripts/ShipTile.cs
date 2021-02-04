﻿using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipTile : BaseTile, IDragHandler, IBeginDragHandler, IEndDragHandler, IPunInstantiateMagicCallback
{
    private Camera GameCamera;

    private PhotonView photonView;

    public byte Id { get; set; }
    public Pirate PirateTemplate;

    public GamePlayer SelfPlayer;
    private BaseTile tempTile;
    public BoxCollider Collider;
    public bool isInFreeSpace = false;

    public List<Pirate> ShipPirates = new List<Pirate>();

    private bool isMyTurn => (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"];

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<BoxCollider>();

        photonView = GetComponent<PhotonView>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = false;

        //Отключаем колайдеры чтобы рейкаст не попадал на модельки, из-за чего ломается перемещение
        foreach (Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        var groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);
            worldPosition.y += 1.5f;

            transform.position = worldPosition;

            //Заменить каждый фрейм вычисляется положение для каждого  пирата по гипер стрёмной формуле
            PlacePirateOnTile();
        }

        //Получение временной ячейки, костыль чтобы запомнить ячейку при отпускании пирата
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<BaseTile>())
        {
            tempTile = eventData.pointerEnter.GetComponent<BaseTile>();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = true;
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<WaterTile>())
        {
            if (TryReplace(tempTile))
            {
                StepByStepSystem.StartNextTurn();
                RaiseEventManager.RaiseReplaceShipEvent(
                new ShipMovementData
                {
                    Id = Id,
                    XPos = tempTile.HorizontalIndex,
                    YPos = tempTile.VerticalIndex
                });
                this.Replace(tempTile);
            }
            else
            {
                this.SetTransformPosition(this.fixedPosition);
            }
        }
        else
        {
            this.SetTransformPosition(this.fixedPosition);
        }

        PlacePirateOnTile();

        foreach (Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = true;
        }
    }

    private bool TryReplace(BaseTile tile)
    {
        return (Mathf.Abs(HorizontalIndex - tile.HorizontalIndex) < 2 &&
            Mathf.Abs(VerticalIndex - tile.VerticalIndex) < 2);
    }

    public void Replace(BaseTile tile)
    {
        byte i = tile.HorizontalIndex;
        byte j = tile.VerticalIndex;

        tile.HorizontalIndex = this.HorizontalIndex;
        tile.VerticalIndex = this.VerticalIndex;
        Map[this.HorizontalIndex][this.VerticalIndex] = tile;
        Map[i][j] = this;

        this.HorizontalIndex = i;
        this.VerticalIndex = j;

        var tempPos = this.fixedPosition;
        this.SetTransformPosition(tile.fixedPosition);
        tile.SetTransformPosition(tempPos);
    }

    public void Move(BaseTile tile)
    {
        this.HorizontalIndex = tile.HorizontalIndex;
        this.VerticalIndex = tile.VerticalIndex;
        this.Map = tile.Map;
        Map[tile.HorizontalIndex][tile.VerticalIndex] = this;

        this.SetTransformPosition(tile.fixedPosition);
        Destroy(tile.gameObject);
        isInFreeSpace = false;
        Collider.enabled = true;

        SelfPlayer = FindObjectOfType<GamePlayer>();

        if(photonView.IsMine)
            this.AddPirateOnTile(3);
    }

    private void AddPirateOnTile(int count)
    {
        for(int i = 0; i < count; i++)
        {
            byte pirateId = (byte)this.ShipPirates.Count;
            Pirate pirate = PhotonNetwork.Instantiate(PirateTemplate.name, this.transform.position, Quaternion.identity, 0, new object[] { pirateId, this.Id }).GetComponent<Pirate>();
            pirate.SelfPlayer = this.SelfPlayer;
        }
    }

    public override void EnterPirate(Pirate pirate)
    {
        base.EnterPirate(pirate);
    }

    public override bool TryAttack(Pirate pirate)
    {
        pirate.Die();
        return false;
    }

    public override void AddCoin(Coin coin)
    {
        Destroy(coin.gameObject);
        if (photonView.IsMine)
        {
            var value = ++this.SelfPlayer.PlayerUI.CoinsCount;
            this.SelfPlayer.PlayerUI.CoinsValue.text = value.ToString();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var mapManger = FindObjectOfType<MapManager>();
        var data = info.photonView.InstantiationData;
        this.Id = (byte)data[0];

        mapManger.ShipTiles.Add(Id, this);
    }
}
