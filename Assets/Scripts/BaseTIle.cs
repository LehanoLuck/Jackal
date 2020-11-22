﻿using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<Pirate> Pirates { get; set; } = new List<Pirate>();

    public BaseTile[][] Map { get; set; }

    public int HorizontalIndex { get; set; }

    public int VerticalIndex { get; set; }

    public int maxSize = 5;

    internal Vector3 fixedPosition;
    protected Vector3 updatePosition;

    public void SetTransformPosition(Vector3 position)
    {
        this.transform.position = position;
        this.fixedPosition = position;
        this.updatePosition = new Vector3(position.x, position.y + 0.25f, position.z);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.position = updatePosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.position = fixedPosition;
    }

    public virtual void EnterPirate(Pirate pirate)
    {
        this.Pirates.Add(pirate);
        SetCurrentPirateTile(pirate);
        PlacePirateOnTile();
    }

    public void LeaveAllPirates()
    {
        this.Pirates.Clear();
    }

    public void LeavePirate(Pirate pirate)
    {
        this.Pirates.Remove(pirate);

        PlacePirateOnTile();
    }

    public void PlacePirateOnTile()
    {
        int count = Pirates.Count;

        //Коэффициент для изменения размера пирата
        float factor = (float)(1 / Math.Pow(Math.Log(Math.E * count), 0.4));
        var localScale = new Vector3(factor, factor, factor);

        float radius = count == 1 ? 0 : this.transform.localScale.x * 0.85f;

        for (int i = 0; i < Pirates.Count; i++)
        {
            Pirates[i].transform.localScale = localScale;

            float value = i * 2f * Mathf.PI / count;

            float x = this.transform.position.x + radius * Mathf.Sin(value);
            float z = this.transform.position.z + radius * Mathf.Cos(value);

            Pirates[i].transform.position = new Vector3(x, this.transform.position.y - 1.25f, z);
        }
    }

    protected virtual void SetCurrentPirateTile(Pirate pirate)
    {
        pirate.CurrentTile = this;
    }
}