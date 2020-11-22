﻿using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipTile : BaseTile, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Camera GameCamera;
    public Pirate PirateTemplate;
    private BaseTile tempTile;
    public BoxCollider Collider;
    public bool isInFreeSpace = false;

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<BoxCollider>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Collider.enabled = false;

        //Отключаем колайдеры чтобы рейкаст не попадал на модельки, из-за чего ломается перемещение
        foreach (Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = false;
        }
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
        Collider.enabled = true;
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<WaterTile>())
        {
            if (!isInFreeSpace)
            {
                if (TryReplace(tempTile))
                {
                    Replace(tempTile);
                }
                else
                {
                    this.SetTransformPosition(this.fixedPosition);
                }
            }
            else
            {
                Move(tempTile);
            }
        }
        else
        {
            this.SetTransformPosition(this.fixedPosition);
        }
        PlacePirateOnTile();

        foreach(Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = true;
        }
    }

    private bool TryReplace(BaseTile tile)
    {
        return (Mathf.Abs(HorizontalIndex - tile.HorizontalIndex) < 2 &&
            Mathf.Abs(VerticalIndex - tile.VerticalIndex) < 2);
    }

    private void Replace(BaseTile tile)
    {
        int i = tile.HorizontalIndex;
        int j = tile.VerticalIndex;

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

        this.AddPirateOnTile(3);
    }

    private void AddPirateOnTile(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Pirate pirate = Instantiate(PirateTemplate, this.transform.parent);
            this.EnterPirate(pirate);
            pirate.ship = this;
        }
    }
}
