using Assets.Scripts.TIles;
using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BasicTile: Tile, IPirateInteractor,IPointerDownHandler
{
    public List<Pirate> Pirates { get; set; } = new List<Pirate>();
    public int MaxPirateSize = 5;
    public bool isHavePirates => Pirates.Count > 0;
    public int ShipId;

    public override void EnterPirate(Pirate pirate)
    {
        if (isHavePirates)
        {
            this.TryAttack(pirate);
        }
        this.AddPirate(pirate);
        this.ShipId = pirate.Ship.Id;

        PlacePirateOnTile();
    }

    public virtual void LeavePirate(Pirate pirate)
    {
        this.Pirates.Remove(pirate);
        PlacePirateOnTile();
    }

    public void LeaveAllPirates()
    {
        this.Pirates.Clear();
    }

    public void KillAllPirates()
    {
        foreach(var pirate in Pirates)
        {
            pirate.Die();
        }
        LeaveAllPirates();
    }

    public virtual void TryAttack(Pirate pirate)
    {
        if (ShipId != pirate.Ship.Id)
        {
            KillAllPirates();

            if (pirate.IsMoveWithCoin())
                pirate.DropCoin();
        }
    }

    public void AddPirate(Pirate pirate)
    {
        pirate.CurrentTile = this;
        this.Pirates.Add(pirate);
    }

    public virtual void PlacePirateOnTile()
    {
        int count = Pirates.Count;

        //Коэффициент для изменения размера пирата
        float factor = (float)(1 / Math.Pow(Math.Log(Math.E * count), 0.4));
        var localScale = new Vector3(factor, factor, factor);

        float radius = count == 1 ? 0 : this.transform.localScale.x * 0.85f;

        for (int i = 0; i < count; i++)
        {
            Pirates[i].transform.localScale = localScale;

            float value = i * 2f * Mathf.PI / count;

            float x = this.transform.position.x + radius * Mathf.Sin(value);
            float z = this.transform.position.z + radius * Mathf.Cos(value);

            Pirates[i].transform.position = new Vector3(x, this.transform.position.y - 1.25f, z);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.pointerDrag && eventData.pointerDrag.GetComponent<Pirate>())
        {
            foreach (var pirate in Pirates)
            {
                pirate.Collider.enabled = false;
            }
        }
        else
        {
            foreach (var pirate in Pirates)
            {
                pirate.Collider.enabled = true;
            }
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        foreach (var pirate in Pirates)
        {
            pirate.Collider.enabled = true;
        }
    }
}
