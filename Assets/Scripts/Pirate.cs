using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Pirate : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Camera GameCamera;

    public MeshCollider Collider;

    private Vector3 StartPosition;

    public BaseTile CurrentTile { get; set; }

    private BaseTile tempTile;

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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider.enabled = true;

        if (eventData.pointerEnter)
        {
            if (TryMoveOnTile(tempTile))
            {
                CurrentTile.LeavePirate(this);
                tempTile.EnterPirate(this);
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
    }

    private bool TryMoveOnTile(BaseTile tile)
    {
        return (CurrentTile != tile &&
            (Math.Abs(tempTile.HorizontalIndex - CurrentTile.HorizontalIndex) < 2) && 
            (Math.Abs(tempTile.VerticalIndex - CurrentTile.VerticalIndex) < 2) && 
            !(tile is WaterTile) &&
            (tempTile.Pirates.Count < tempTile.maxSize));
    }
}
