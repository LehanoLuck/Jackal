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

    MeshCollider Collider;

    private Vector3 StartPosition;

    public VoxelTile CurrentTile { get; set; }

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

        if (eventData.pointerEnter)
        {
            var tile = eventData.pointerEnter.GetComponent<VoxelTile>();
            if(Math.Abs(tile.HorizontalIndex - CurrentTile.HorizontalIndex) < 2 && Math.Abs(tile.VerticalIndex - CurrentTile.VerticalIndex) < 2)
            {
                var placePosition = new Vector3(eventData.pointerEnter.transform.position.x, eventData.pointerEnter.transform.position.y - 1.25f, eventData.pointerEnter.transform.position.z);
                this.transform.position = placePosition;
                StartPosition = this.transform.position;
                CurrentTile = tile;
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
}
